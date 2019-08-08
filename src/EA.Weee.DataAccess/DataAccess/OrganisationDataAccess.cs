namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using Domain.User;

    public class OrganisationDataAccess : IOrganisationDataAccess
    {
        private readonly WeeeContext context;

        public OrganisationDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Organisation> GetBySchemeId(Guid schemeId)
        {
            var scheme = await context.Schemes
                .Include(s => s.Organisation)
                .SingleAsync(s => s.Id == schemeId);

            return scheme.Organisation;
        }

        public async Task<Organisation> GetById(Guid organisationId)
        {
            var organisation = await context.Organisations
                .SingleAsync(c => c.Id == organisationId);

            return organisation;
        }

        public async Task Delete(Guid organisationId)
        {
            var organisation = await context.Organisations.FirstOrDefaultAsync(p => p.Id == organisationId);

            if (organisation == null)
            {
                throw new ArgumentException($"Organisation not found with id {organisationId}");
            }

            context.OrganisationUsers.RemoveRange(context.OrganisationUsers.Where(o => o.OrganisationId == organisation.Id));

            context.Entry(organisation).State = EntityState.Deleted;

            await context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveUsers(Guid organisationId)
        {
            return await context.OrganisationUsers
                .AnyAsync(p => p.OrganisationId == organisationId && p.UserStatus.Value == UserStatus.Active.Value);
        }

        public async Task<bool> HasReturns(Guid organisationId, int year)
        {
            return await context.Returns.AnyAsync(r => r.Organisation.Id == organisationId && r.Quarter.Year == year);
        }
    }
}
