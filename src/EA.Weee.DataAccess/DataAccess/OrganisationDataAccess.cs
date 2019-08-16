namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
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

            foreach (var organisationUser in context.OrganisationUsers.Where(o => o.OrganisationId == organisation.Id))
            {
                context.Entry(organisationUser).State = EntityState.Deleted;
            }

            context.Entry(organisation).State = EntityState.Deleted;

            await context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveUsers(Guid organisationId)
        {
            return await context.OrganisationUsers
                .AnyAsync(p => p.OrganisationId == organisationId && p.UserStatus.Value == UserStatus.Active.Value);
        }

        public async Task<bool> HasScheme(Guid organisationId)
        {
            return await context.Schemes
                .AnyAsync(p => p.OrganisationId == organisationId);
        }

        public async Task<bool> HasFacility(Guid organisationId, FacilityType facilityType)
        {
            return await context.Aatfs
                .AnyAsync(p => p.Organisation.Id == organisationId && p.FacilityType.Value == facilityType.Value);
        }

        public async Task<bool> HasReturns(Guid organisationId, int year)
        {
            return await context.Returns.AnyAsync(r => r.Organisation.Id == organisationId && r.Quarter.Year == year);
        }

        public async Task<List<Return>> GetReturnsByComplianceYear(Guid organisationId, int year)
        {
            return await context.Returns.Where(r => r.Quarter.Year == year && r.Organisation.Id == organisationId).ToListAsync();
        }
    }
}
