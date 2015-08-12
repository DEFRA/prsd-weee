namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class UpdateSchemeInformationHandler : IRequestHandler<UpdateSchemeInformation, Guid>
    {
        private readonly WeeeContext db;

        public UpdateSchemeInformationHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<Guid> HandleAsync(UpdateSchemeInformation message)
        {
            var scheme = await db.Schemes.SingleOrDefaultAsync(o => o.Id == message.SchemeId);

            if (scheme == null)
            {
                throw new ArgumentException(string.Format("Could not find a scheme with id {0}",
                    message.SchemeId));
            }

            var obligationType = ValueObjectInitializer.GetObligationType(message.ObligationType);

            scheme.UpdateScheme(message.SchemeName, message.ApprovalNumber, message.IbisCustomerReference, obligationType, message.CompetentAuthorityId);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return scheme.Id;
        }
    }
}
