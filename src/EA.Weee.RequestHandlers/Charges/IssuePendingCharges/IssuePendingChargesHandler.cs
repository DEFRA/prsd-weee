namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using Security;
    using Shared.DomainUser;

    public class IssuePendingChargesHandler : IRequestHandler<Requests.Charges.IssuePendingCharges, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IIssuePendingChargesDataAccess dataAccess;
        private readonly IIbisFileDataGenerator ibisFileDataGenerator;
        private readonly IDomainUserContext domainUserContext;

        public IssuePendingChargesHandler(
            IWeeeAuthorization authorization,
            IIssuePendingChargesDataAccess dataAccess,
            IIbisFileDataGenerator ibisFileDataGenerator,
            IDomainUserContext domainUserContext)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.ibisFileDataGenerator = ibisFileDataGenerator;
            this.domainUserContext = domainUserContext;
        }

        public async Task<Guid> HandleAsync(Requests.Charges.IssuePendingCharges message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IReadOnlyList<MemberUpload> memberUploads = await dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(authority);

            User issuingUser = await domainUserContext.GetCurrentUserAsync();

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, issuingUser);

            if (authority.Name == "Environment Agency")
            {
                ulong fileId = await GetNextIbisFileId();

                IbisFileData ibisFileData = await ibisFileDataGenerator.CreateFileDataAsync(fileId, invoiceRun);

                invoiceRun.SetIbisFileData(ibisFileData);
            }

            await dataAccess.SaveAsync(invoiceRun);

            return invoiceRun.Id;
        }

        public async Task<ulong> GetNextIbisFileId()
        {
            ulong? currentMaximumFileID = await dataAccess.GetMaximumIbisFileIdOrDefaultAsync();

            if (currentMaximumFileID != null)
            {
                return currentMaximumFileID.Value + 1;
            }
            else
            {
                return await dataAccess.GetInitialIbisFileIdAsync();
            }
        }
    }
}
