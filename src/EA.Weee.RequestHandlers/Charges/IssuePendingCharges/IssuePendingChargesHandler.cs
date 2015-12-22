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

        public IssuePendingChargesHandler(
            IWeeeAuthorization authorization,
            IIssuePendingChargesDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<Guid> HandleAsync(Requests.Charges.IssuePendingCharges message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IList<MemberUpload> memberUploads = await dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(authority);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);

            await dataAccess.SaveAsync(invoiceRun);

            return invoiceRun.Id;
        }
    }
}
