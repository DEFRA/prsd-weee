namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Domain.Auditing;
    using EA.Weee.DataAccess;
    using System;
    using System.Threading.Tasks;

    public class GetReturnValueFromRemovingPCSRecordsDataAccess : IGetReturnValueFromRemovingPCSRecordsDataAccess
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public GetReturnValueFromRemovingPCSRecordsDataAccess(WeeeContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<int> GetItemsAsync(Guid schemeId, int complianceYear)
        {
            var resultVal = await context.StoredProcedures.SpgRemovePCSRecords(schemeId, complianceYear);

            if (resultVal == 0)
            {
                var userId = userContext.UserId;
                var strOriginalValue = "{'SchemeId':" + schemeId + ",'ComplianceYear':" + complianceYear + "}";
                var auditLog = new AuditLog(userId, SystemTime.UtcNow, EventType.Deleted, "[PCS].[RemovePCSRecord]", schemeId, strOriginalValue, null);
                context.Set<AuditLog>().Add(auditLog);

                await context.SaveChangesAsync();
            }

            return resultVal;
        }
    }
}
