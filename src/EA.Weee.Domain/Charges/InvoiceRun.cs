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
        public UKCompetentAuthority CompetentAuthority { get; set; }

        public IReadOnlyCollection<MemberUpload> MemberUploads { get; set; }

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
