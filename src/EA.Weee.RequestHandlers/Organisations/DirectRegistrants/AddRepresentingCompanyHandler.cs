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

        public AddRepresentingCompanyHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<Guid> HandleAsync(AddRepresentingCompany request)
        { 
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            var existingDirectRegistrant =
                await weeeContext.DirectRegistrants.Include(directRegistrant1 => directRegistrant1.Organisation)
                    .Include(directRegistrant2 => directRegistrant2.BrandName)
                    .Include(directRegistrant3 => directRegistrant3.Contact)
                    .Include(directRegistrant4 => directRegistrant4.Address)
                    .Include(directRegistrant5 => directRegistrant5.AdditionalCompanyDetails).FirstOrDefaultAsync(d =>
                    d.OrganisationId == request.OrganisationId);

            if (existingDirectRegistrant == null)
            {
                throw new ArgumentNullException(
                    $"AddRepresentingCompanyHandler no existing direct registrant found for organisation {request.OrganisationId}");
            }

            var representingCompany = await CreateRepresentingCompany(request.RepresentingCompanyDetailsViewModel);

            var directRegistrant = DirectRegistrant.CreateDirectRegistrant(existingDirectRegistrant.Organisation, existingDirectRegistrant.BrandName, existingDirectRegistrant.Contact, existingDirectRegistrant.Address, representingCompany, existingDirectRegistrant.AdditionalCompanyDetails.ToList());

            await genericDataAccess.Add(directRegistrant);

            await weeeContext.SaveChangesAsync();

            return directRegistrant.Id;
        }

        private async Task<AuthorisedRepresentative> CreateRepresentingCompany(RepresentingCompanyDetailsViewModel representingCompanyDetails)
        {
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == representingCompanyDetails.Address.CountryId);

            return RepresentingCompanyHelper.CreateRepresentingCompany(representingCompanyDetails, country);
        }
    }
}
