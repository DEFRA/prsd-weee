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
            if (CorrespondentForNoticesContact == null && producerBusinessObj.CorrespondentForNoticesContact == null)
            {
                isCorrespondentContactSame = true;
            }
            else
            {
                if (CorrespondentForNoticesContact != null && producerBusinessObj.CorrespondentForNoticesContact != null)
                {
                    isCorrespondentContactSame =
                        CorrespondentForNoticesContact.Equals(producerBusinessObj.CorrespondentForNoticesContact);
                }
            }

            //compare company details
            var compareCompany = false;
            if (CompanyDetails == null && producerBusinessObj.CompanyDetails == null)
            {
                compareCompany = true;
            }
            else
            {
                if (CompanyDetails != null && producerBusinessObj.CorrespondentForNoticesContact != null)
                {
                    compareCompany = CompanyDetails.Equals(producerBusinessObj.CompanyDetails);
                }
            }
            
            //compare partnership details
            var comparePartnership = false;

            if (Partnership == null && producerBusinessObj.Partnership == null)
            {
                comparePartnership = true;
            }
            else
            {
                if (Partnership != null && producerBusinessObj.Partnership != null)
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