namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using Security;

    public class IssuePendingChargesHandler : IRequestHandler<Requests.Charges.IssuePendingCharges, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IIssuePendingChargesDataAccess dataAccess;
        private readonly IIbisFileDataGenerator ibisFileDataGenerator;

        public IssuePendingChargesHandler(
            IWeeeAuthorization authorization,
            IIssuePendingChargesDataAccess dataAccess,
            IIbisFileDataGenerator ibisFileGenerator)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.ibisFileDataGenerator = ibisFileGenerator;
        }

        public async Task<Guid> HandleAsync(Requests.Charges.IssuePendingCharges message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IReadOnlyList<MemberUpload> memberUploads = await dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(authority);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);

            if (authority.Name == "Environment Agency")
            {
                ulong fileId = await GetNextIbisFileId();

                IbisFileData ibisFileData = await ibisFileDataGenerator.CreateFileDataAsync(fileId, memberUploads);

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
