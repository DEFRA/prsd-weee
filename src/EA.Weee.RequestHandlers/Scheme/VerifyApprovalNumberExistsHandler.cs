namespace EA.Weee.RequestHandlers.Scheme
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class VerifyApprovalNumberExistsHandler : IRequestHandler<VerifyApprovalNumberExists, bool>
    {
        private readonly WeeeContext db;

        public VerifyApprovalNumberExistsHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<bool> HandleAsync(VerifyApprovalNumberExists message)
        {
            var isExists = await db.Schemes.AnyAsync(o => o.ApprovalNumber == message.ApprovalNumber);

            return isExists;
        }
    }
}
