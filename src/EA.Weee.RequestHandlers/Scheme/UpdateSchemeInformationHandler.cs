namespace EA.Weee.RequestHandlers.Scheme
{
    using Core.Helpers;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class UpdateSchemeInformationHandler : IRequestHandler<UpdateSchemeInformation, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public UpdateSchemeInformationHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateSchemeInformation message)
        {
            authorization.EnsureCanAccessInternalArea();

            var scheme = await db.Schemes.FirstOrDefaultAsync(o => o.Id == message.SchemeId);

            if (scheme == null)
            {
                throw new ArgumentException(string.Format("Could not find a scheme with id {0}",
                    message.SchemeId));
            }

            if (scheme.ApprovalNumber != message.ApprovalNumber)
            {
                var verifyApprovalNumberExistsHandler = new VerifyApprovalNumberExistsHandler(db, authorization);
                var isExists = await verifyApprovalNumberExistsHandler.ApprovalNumberExists(message.ApprovalNumber);
                if (isExists)
                {
                    throw new Exception(string.Format("Approval number {0} already exists.", message.ApprovalNumber));
                }
            }

            var obligationType = ValueObjectInitializer.GetObligationType(message.ObligationType);

            scheme.UpdateScheme(message.SchemeName, message.ApprovalNumber, message.IbisCustomerReference, obligationType, message.CompetentAuthorityId);
            scheme.SetStatus(message.Status.ToDomainEnumeration<SchemeStatus>());

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
