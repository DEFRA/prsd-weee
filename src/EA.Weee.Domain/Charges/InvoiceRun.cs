namespace EA.Weee.Domain.Charges
{
    using System;
    using System.Collections.Generic;
    using Domain.Scheme;
    using Domain.User;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    /// <summary>
    /// An invoice run is a collection of member uploads which have been grouped
    /// together for processing.
    /// </summary>
    public class InvoiceRun : Entity
    {
        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public virtual DateTime IssuedDate { get; private set; }

        public User IssuedByUser { get; private set; }

        public IReadOnlyList<MemberUpload> MemberUploads { get; private set; }

        public virtual IbisFileData IbisFileData { get; private set; }

        public InvoiceRun(
            UKCompetentAuthority competentAuthority,
            IReadOnlyList<MemberUpload> memberUploads,
            User issuingUser)
        {
            Guard.ArgumentNotNull(() => competentAuthority, competentAuthority);
            Guard.ArgumentNotNull(() => memberUploads, memberUploads);
            Guard.ArgumentNotNull(() => issuingUser, issuingUser);

            CompetentAuthority = competentAuthority;
            IssuedDate = SystemTime.UtcNow;
            IssuedByUser = issuingUser;

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

        public void SetIbisFileData(IbisFileData ibisFileData)
        {
            Guard.ArgumentNotNull(() => ibisFileData, ibisFileData);

            if (CompetentAuthority.Name != "Environment Agency")
            {
                string errorMessage = "1B1S files can only be provided for the Environment Agency. Devolved agencies do not use 1B1S.";
                throw new InvalidOperationException(errorMessage);
            }

            if (IbisFileData != null)
            {
                string errorMessage = "Once 1B1S files have been provided for an invoice run, they cannot be replaced.";
                throw new InvalidOperationException(errorMessage);
            }

            IbisFileData = ibisFileData;
        }
    }
}
