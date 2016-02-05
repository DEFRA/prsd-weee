namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Obligation;
    using Organisation;
    using Producer;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Scheme : Entity
    {
        public Scheme(Guid organisationId)
        {
            OrganisationId = organisationId;
            SchemeStatus = SchemeStatus.Pending;
            ApprovalNumber = string.Empty;
        }

        public Scheme(Organisation organisation)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);

            Organisation = organisation;
            OrganisationId = organisation.Id;
            SchemeStatus = SchemeStatus.Pending;
            ApprovalNumber = string.Empty;
        }

        protected Scheme()
        {
        }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual SchemeStatus SchemeStatus { get; private set; }

        public virtual string ApprovalNumber { get; private set; }

        public virtual string SchemeName { get; private set; }

        public string IbisCustomerReference { get; private set; }

        public ObligationType? ObligationType { get; private set; }

        public Guid? CompetentAuthorityId { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public void UpdateScheme(
            string schemeName,
            string approvalNumber,
            string ibisCustomerReference,
            ObligationType? obligationType,
            Guid competentAuthorityId)
        {
            Guard.ArgumentNotNullOrEmpty(() => schemeName, schemeName);
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);

            SchemeName = schemeName;
            ApprovalNumber = approvalNumber;
            IbisCustomerReference = ibisCustomerReference;
            ObligationType = obligationType;
            CompetentAuthorityId = competentAuthorityId;
        }

        public void UpdateScheme(
            string schemeName,
            string approvalNumber,
            string ibisCustomerReference,
            ObligationType? obligationType,
            UKCompetentAuthority competentAuthority)
        {
            Guard.ArgumentNotNullOrEmpty(() => schemeName, schemeName);
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNull(() => competentAuthority, competentAuthority);

            SchemeName = schemeName;
            ApprovalNumber = approvalNumber;
            IbisCustomerReference = ibisCustomerReference;
            ObligationType = obligationType;
            CompetentAuthority = competentAuthority;
            CompetentAuthorityId = competentAuthority.Id;
        }

        public void SetStatus(SchemeStatus status)
        {
            if ((SchemeStatus == SchemeStatus.Withdrawn && status != SchemeStatus.Withdrawn)
                || (SchemeStatus == SchemeStatus.Rejected && status != SchemeStatus.Rejected))
            {
                throw new InvalidOperationException(
                    string.Format("Scheme cannot transition scheme status '{0}' to '{1}'", SchemeStatus, status));
            }

            if (SchemeStatus == SchemeStatus.Pending && status == SchemeStatus.Withdrawn)
            {
                throw new InvalidOperationException(
                    string.Format("Scheme cannot transition scheme status '{0}' to '{1}'", SchemeStatus, status));
            }

            if (SchemeStatus == SchemeStatus.Approved && (status == SchemeStatus.Pending || status == SchemeStatus.Rejected))
            {
                throw new InvalidOperationException(
                    string.Format("Scheme cannot transition scheme status '{0}' to '{1}'", SchemeStatus, status));
            }

            if (SchemeStatus == SchemeStatus.Withdrawn && status == SchemeStatus.Withdrawn)
            {
                throw new InvalidOperationException(
                    string.Format("Scheme cannot transition scheme status '{0}' to '{1}'", SchemeStatus, status));
            }

            SchemeStatus = status;
        }
    }
}