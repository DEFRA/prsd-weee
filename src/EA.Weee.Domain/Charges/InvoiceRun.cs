namespace EA.Weee.Domain.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Scheme;

    /// <summary>
    /// An invoice run is a collection of member uploads which have been grouped
    /// together for processing.
    /// </summary>
    public class InvoiceRun : Entity
    {
        public UKCompetentAuthority CompetentAuthority { get; private set; }

        public IReadOnlyCollection<MemberUpload> MemberUploads { get; private set; }

        public InvoiceRun(UKCompetentAuthority competentAuthority, IList<MemberUpload> memberUploads)
        {
            Guard.ArgumentNotNull(() => competentAuthority, competentAuthority);
            Guard.ArgumentNotNull(() => memberUploads, memberUploads);

            CompetentAuthority = competentAuthority;

            if (memberUploads.Count == 0)
            {
                string errorMessage = "An invoice run must contain at least one member upload.";
                throw new InvalidOperationException(errorMessage);
            }

            foreach (MemberUpload memberUpload in memberUploads)
            {
                if (memberUpload.Scheme.CompetentAuthority != competentAuthority)
                {
                    string errorMessage = "All member uploads assigned to an invoice run must be related " +
                        "to schemes with the same authority as the invoice run.";
                    throw new InvalidOperationException(errorMessage);
                }

                memberUpload.AssignToInvoiceRun(this);
            }

            MemberUploads = new List<MemberUpload>(memberUploads);
        }

        /// <summary>
        /// This constructor is used only by Entity Framework.
        /// </summary>
        protected InvoiceRun()
        {
        }
    }
}
