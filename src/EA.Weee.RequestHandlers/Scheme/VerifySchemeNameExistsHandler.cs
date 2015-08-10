namespace EA.Weee.RequestHandlers.Scheme
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class VerifySchemeNameExistsHandler : IRequestHandler<VerifySchemeNameExists, bool>
    {
        private readonly WeeeContext db;

        public VerifySchemeNameExistsHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<bool> HandleAsync(VerifySchemeNameExists message)
        {
            var isExists = await db.Schemes.AnyAsync(o => o.SchemeName == message.SchemeName);

            return isExists;
        }
    }
}
