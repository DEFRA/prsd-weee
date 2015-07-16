namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class ProducerBusiness : Entity
    {
        public ProducerBusiness(Company companyDetails = null, Partnership partnership = null, ProducerContact correspondentForNoticesContact = null)
        {
            CompanyDetails = companyDetails;
            Partnership = partnership;
            CorrespondentForNoticesContact = correspondentForNoticesContact;
        }

         protected ProducerBusiness()
        {
        }

         public Guid? CorrespondentForNoticesContactId { get; private set; }
        public virtual ProducerContact CorrespondentForNoticesContact { get; private set; }

        public Guid? CompanyId { get; private set; }
        public virtual Company CompanyDetails { get; private set; }

        public Guid? PartnershipId { get; private set; }
        public virtual Partnership Partnership { get; private set; }
    }
}