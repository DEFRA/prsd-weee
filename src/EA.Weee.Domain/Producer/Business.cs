namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class ProducerBusiness : Entity
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

        public override bool Equals(object obj)
        {
            var producerBusinessObj = obj as ProducerBusiness;

            if (producerBusinessObj == null)
            {
                return false;
            }
            var isCorrespondentContactSame = false;
            if (producerBusinessObj.CorrespondentForNoticesContact != null)
            {
                isCorrespondentContactSame =
                    CorrespondentForNoticesContact.Equals(producerBusinessObj.CorrespondentForNoticesContact);
            }

            var compareCompany = true;
            var comparePartnership = true;
            if (CompanyDetails != null || Partnership != null)
            {
                if (CompanyDetails != null)
                {
                    compareCompany = CompanyDetails.Equals(producerBusinessObj.CompanyDetails);
                }

                if (Partnership != null)
                {
                    comparePartnership = Partnership.Equals(producerBusinessObj.Partnership);
                }
            }
            return isCorrespondentContactSame &&
                   compareCompany && comparePartnership;
        }

        public Guid? CorrespondentForNoticesContactId { get; private set; }

        public virtual ProducerContact CorrespondentForNoticesContact { get; private set; }

        public Guid? CompanyId { get; private set; }

        public virtual Company CompanyDetails { get; private set; }

        public Guid? PartnershipId { get; private set; }

        public virtual Partnership Partnership { get; private set; }
    }
}