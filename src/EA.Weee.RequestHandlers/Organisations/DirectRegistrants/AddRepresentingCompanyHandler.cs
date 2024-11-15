namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class AddRepresentingCompanyHandler : IRequestHandler<AddRepresentingCompany, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        public AddRepresentingCompanyHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext, ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.smallProducerDataAccess = smallProducerDataAccess;
        }

        public async Task<Guid> HandleAsync(AddRepresentingCompany request)
        { 
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            var existingDirectRegistrant =
                await smallProducerDataAccess.GetDirectRegistrantByOrganisationId(request.OrganisationId);

            if (existingDirectRegistrant == null)
            {
                throw new ArgumentNullException(
                    $"AddRepresentingCompanyHandler no existing direct registrant found for organisation {request.OrganisationId}");
            }

            var representingCompany = await CreateRepresentingCompany(request.RepresentingCompanyDetailsViewModel);

            // here if the current one is null set it
            if (existingDirectRegistrant.AuthorisedRepresentative == null)
            {
                existingDirectRegistrant.AddOrUpdateAuthorisedRepresentitive(representingCompany);

                return existingDirectRegistrant.Id;
            }

            var directRegistrant = DirectRegistrant.CreateDirectRegistrant(existingDirectRegistrant.Organisation, existingDirectRegistrant.BrandName, existingDirectRegistrant.Contact, existingDirectRegistrant.Address, representingCompany, existingDirectRegistrant.AdditionalCompanyDetails.ToList(), null);

            var newRegistrant = await genericDataAccess.Add(directRegistrant);

            return newRegistrant.Id;
        }

        private async Task<AuthorisedRepresentative> CreateRepresentingCompany(RepresentingCompanyDetailsViewModel representingCompanyDetails)
        {
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == representingCompanyDetails.Address.CountryId);

            return RepresentingCompanyHelper.CreateRepresentingCompany(representingCompanyDetails, country);
        }
    }
}
