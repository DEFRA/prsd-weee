namespace EA.Weee.Domain.Producer
{
    using System.Collections.Generic;
    using System.Reflection;
    using Prsd.Core.Domain;

    public class Business : Entity
    {
        public Business(Company companyDetails, Partnership partnership, ProducerContact correspondenceForNoticesContact = null)
        {
            CompanyDetails = companyDetails;
            Partnership = partnership;
            CorrespondenceForNoticesContact = correspondenceForNoticesContact;
        }

         protected Business()
        {
        }

        public virtual ProducerContact CorrespondenceForNoticesContact { get; private set; }

        public virtual Company CompanyDetails { get; private set; }

        public virtual Partnership Partnership { get; private set; }
    }
}