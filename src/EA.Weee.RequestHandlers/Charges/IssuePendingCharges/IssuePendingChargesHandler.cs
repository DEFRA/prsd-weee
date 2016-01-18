namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Charges;
    using Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using Security;
    using Shared.DomainUser;

    public class IssuePendingChargesHandler : IRequestHandler<Requests.Charges.IssuePendingCharges, IssuePendingChargesResult>
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

        public async Task<IssuePendingChargesResult> HandleAsync(Requests.Charges.IssuePendingCharges message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IReadOnlyList<MemberUpload> memberUploads = await dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(authority);

            User issuingUser = await domainUserContext.GetCurrentUserAsync();

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, issuingUser);

            var result = new IssuePendingChargesResult();
            result.Errors = new List<string>();

            if (authority.Name == "Environment Agency")
            {
                ulong fileId = await GetNextIbisFileId();

                IbisFileDataGeneratorResult ibisFileDataGeneratorResult = await ibisFileDataGenerator.CreateFileDataAsync(fileId, invoiceRun);

                result.Errors.AddRange(ibisFileDataGeneratorResult.Errors);
                if (result.Errors.Count == 0)
                {
                    IbisFileData ibisFileData = ibisFileDataGeneratorResult.IbisFileData;
                    invoiceRun.SetIbisFileData(ibisFileData);
                }
            }

            if (result.Errors.Count == 0)
            {
                await dataAccess.SaveAsync(invoiceRun);
                result.InvoiceRunId = invoiceRun.Id;
            }

            return result;
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
