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
    using Weee.Security;

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

            List<ProducerSubmission> producers = await dataAccess.Fetch(request.RegistrationNumber, request.ComplianceYear);

            if (producers.Count == 0)
            {
                string message = string.Format(
                    "No producer has been registered with the registration number \"{0}\" in the {1} compliance year.",
                    request.RegistrationNumber, request.ComplianceYear);

                throw new ArgumentException(message);
            }

            ProducerDetails producerDetails = new ProducerDetails();
            producerDetails.CanRemoveProducer = authorization.CheckUserInRole(Roles.InternalAdmin);
            producerDetails.Schemes = new List<ProducerDetailsScheme>();

            var schemeGroups = producers
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

                ProducerContact producerBusinessContact = null;
                bool isCompany = false;
                if (latestDetails.ProducerBusiness.CompanyDetails != null)
                {
                    producerBusinessContact = latestDetails.ProducerBusiness.CompanyDetails.RegisteredOfficeContact;
                    isCompany = true;
                }
                else if (latestDetails.ProducerBusiness.Partnership != null)
                {
                    producerBusinessContact = latestDetails.ProducerBusiness.Partnership.PrincipalPlaceOfBusiness;
                }

                ProducerContactDetails producerBusinessContactDetails = null;
                if (producerBusinessContact != null)
                {
                    producerBusinessContactDetails = new ProducerContactDetails
                    {
                        ContactName = producerBusinessContact.ContactName,
                        Email = producerBusinessContact.Email,
                        Mobile = producerBusinessContact.Mobile,
                        Telephone = producerBusinessContact.Telephone,
                        Address = producerBusinessContact.Address.ToString()
                    };
                }

                ProducerContactDetails correspondentForNotices = null;
                if (latestDetails.ProducerBusiness.CorrespondentForNoticesContact != null)
                {
                    var correspondentDetails = latestDetails.ProducerBusiness.CorrespondentForNoticesContact;

                    correspondentForNotices = new ProducerContactDetails
                    {
                        ContactName = correspondentDetails.ContactName,
                        Email = correspondentDetails.Email,
                        Mobile = correspondentDetails.Mobile,
                        Telephone = correspondentDetails.Telephone,
                        Address = correspondentDetails.Address.ToString()
                    };
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
                    ProducerBusinessContact = producerBusinessContactDetails,
                    IsCompany = isCompany,
                    CorrespondentForNotices = correspondentForNotices,
                    ProducerEeeDetails =
                        mapper.Map<IEnumerable<ProducerEeeByQuarter>, ProducerEeeDetails>(
                            (await
                                dataAccess.EeeOutputBySchemeAndComplianceYear(request.RegistrationNumber, request.ComplianceYear,
                                    schemeGroup.Key.Id)))
                };

                producerDetails.Schemes.Add(producerSchemeDetails);
            }

            return producerDetails;
        }
    }
}
