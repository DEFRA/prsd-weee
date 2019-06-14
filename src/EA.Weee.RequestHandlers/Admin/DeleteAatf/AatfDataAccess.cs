namespace EA.Weee.RequestHandlers.Admin.DeleteAatf
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class AatfDataAccess : IAatfDataAccess
    {
        private readonly WeeeContext context;

        public AatfDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<bool> DoesAatfHaveData(Guid aatfId)
        {
            return await context.WeeeSentOn.CountAsync(p => p.AatfId == aatfId) > 0
                || await context.WeeeReused.CountAsync(p => p.AatfId == aatfId) > 0
                || await context.WeeeReceived.CountAsync(p => p.AatfId == aatfId) > 0;
        }

        public async Task<bool> DoesAatfOrganisationHaveActiveUsers(Guid aatfId)
        {
            Aatf aatf = await this.GetAatfById(aatfId);

            Guid organisationId = aatf.Organisation.Id;

            return await context.OrganisationUsers.CountAsync(p => p.OrganisationId == organisationId) > 0;
        }

        public async Task<bool> DoesAatfOrganisationHaveMoreAatfs(Guid aatfId)
        {
            Aatf aatf = await this.GetAatfById(aatfId);

            Guid organisationId = aatf.Organisation.Id;

            return await context.Aatfs.CountAsync(p => p.Organisation.Id == organisationId) > 1;
        }

        private async Task<Aatf> GetAatfById(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
