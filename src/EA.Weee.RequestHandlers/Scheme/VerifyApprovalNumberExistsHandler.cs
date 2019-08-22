namespace EA.Weee.RequestHandlers.Scheme
{
    using DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class VerifyApprovalNumberExistsHandler : IRequestHandler<VerifyApprovalNumberExists, bool>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public VerifyApprovalNumberExistsHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(VerifyApprovalNumberExists message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await ApprovalNumberExists(message.ApprovalNumber);
        }

        internal async Task<bool> ApprovalNumberExists(string approvalNumber)
        {
            return await db.Schemes.AnyAsync(o => o.ApprovalNumber == approvalNumber);
        }
    }
}
