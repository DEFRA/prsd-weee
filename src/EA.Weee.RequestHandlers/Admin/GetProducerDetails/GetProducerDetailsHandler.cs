namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;

    public class GetProducerDetailsHandler : IRequestHandler<Requests.Admin.GetProducerDetails, ProducerDetails>
    {
        private readonly IGetProducerDetailsDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;

        public GetProducerDetailsHandler(IGetProducerDetailsDataAccess dataAccess, IWeeeAuthorization authorization, IMapper mapper)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<ProducerDetails> HandleAsync(Requests.Admin.GetProducerDetails request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<ProducerSubmission> producers = await dataAccess.Fetch(request.RegistrationNumber);

            if (producers.Count == 0)
            {
                string message = string.Format(
                    "No producer has been registered with the registration number \"{0}\".",
                    request.RegistrationNumber);

                throw new Exception(message);
            }

            int complianceYear = producers.Max(p => p.MemberUpload.ComplianceYear.Value);

            ProducerDetails producerDetails = new ProducerDetails();
            producerDetails.RegistrationNumber = request.RegistrationNumber;
            producerDetails.ComplianceYear = complianceYear;
            producerDetails.Schemes = new List<ProducerDetailsScheme>();

            var schemeGroups = producers
                .Where(p => p.MemberUpload.ComplianceYear.Value == complianceYear)
                .GroupBy(p => new { p.MemberUpload.Scheme.Id, p.MemberUpload.Scheme.SchemeName })
                .OrderBy(p => p.Key.SchemeName);

            foreach (var schemeGroup in schemeGroups)
            {
                DateTime registrationDate = schemeGroup
                    .OrderBy(p => p.UpdatedDate)
                    .First()
                    .UpdatedDate;

                ProducerSubmission latestDetails = schemeGroup
                    .OrderBy(p => p.UpdatedDate)
                    .Last();

                string companyNumber = null;
                if (latestDetails.ProducerBusiness.CompanyDetails != null)
                {
                    companyNumber = latestDetails.ProducerBusiness.CompanyDetails.CompanyNumber;
                }

                var isAuthorisedRepresentative = "No";

                if (latestDetails.AuthorisedRepresentativeId != null &&
                    latestDetails.AuthorisedRepresentative.OverseasContactId != null &&
                    latestDetails.AuthorisedRepresentative.OverseasContact != null &&
                    !string.IsNullOrEmpty(latestDetails.AuthorisedRepresentative.OverseasContact.Email))
                {
                    isAuthorisedRepresentative = "Yes";
                }

                ProducerDetailsScheme producerSchemeDetails = new ProducerDetailsScheme()
                {
                    RegisteredProducerId = latestDetails.RegisteredProducer.Id,
                    SchemeName = schemeGroup.Key.SchemeName,
                    ProducerName = latestDetails.OrganisationName,
                    TradingName = latestDetails.TradingName,
                    RegistrationDate = registrationDate,
                    CompanyNumber = companyNumber,
                    ObligationType = (ObligationType)latestDetails.ObligationType,
                    ChargeBandType = (ChargeBandType)latestDetails.ChargeBandAmount.ChargeBand,
                    CeasedToExist = latestDetails.CeaseToExist,
                    IsAuthorisedRepresentative = isAuthorisedRepresentative,
                    ProducerEeeDetails =
                        mapper.Map<IEnumerable<ProducerEeeByQuarter>, ProducerEeeDetails>(
                            (await
                                dataAccess.EeeOutputBySchemeAndComplianceYear(request.RegistrationNumber, complianceYear,
                                    schemeGroup.Key.Id)))
                };

                producerDetails.Schemes.Add(producerSchemeDetails);
            }

            return producerDetails;
        }
    }
}
