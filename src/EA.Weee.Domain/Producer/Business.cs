namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;
    using System;

    public class ProducerBusiness : Entity, IEquatable<ProducerBusiness>
    {
        public ProducerBusiness(Company companyDetails = null, Partnership partnership = null,
            ProducerContact correspondentForNoticesContact = null)
        {
            CompanyDetails = companyDetails;
            Partnership = partnership;
            CorrespondentForNoticesContact = correspondentForNoticesContact;
        }

        protected ProducerBusiness()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool Equals(ProducerBusiness other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(CompanyDetails, other.CompanyDetails) &&
                   object.Equals(Partnership, other.Partnership) &&
                   object.Equals(CorrespondentForNoticesContact, other.CorrespondentForNoticesContact);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProducerBusiness);
        }

        public Guid? CorrespondentForNoticesContactId { get; private set; }

        public virtual ProducerContact CorrespondentForNoticesContact { get; private set; }

        public Guid? CompanyId { get; private set; }

        public virtual Company CompanyDetails { get; private set; }

        public Guid? PartnershipId { get; private set; }

        public virtual Partnership Partnership { get; private set; }
    }
}