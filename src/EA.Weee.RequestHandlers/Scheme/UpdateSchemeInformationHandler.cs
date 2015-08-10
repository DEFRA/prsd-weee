namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
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

            scheme.UpdateScheme(message.SchemeName, message.IbisCustomerReference, GetObligationType(message.ObligationType), message.CompetentAuthorityId);

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

        public ObligationType GetObligationType(Core.Scheme.MemberUploadTesting.ObligationType obligationType)
        {
            switch (obligationType)
            {
                case Core.Scheme.MemberUploadTesting.ObligationType.B2B:
                    return ObligationType.B2B;

                case Core.Scheme.MemberUploadTesting.ObligationType.B2C:
                    return ObligationType.B2C;

                case Core.Scheme.MemberUploadTesting.ObligationType.Both:
                    return ObligationType.Both;

                default:
                    throw new ArgumentException(string.Format("Unknown obligation type: {0}", obligationType),
                        "obligationType");
            }
        }
    }
}
