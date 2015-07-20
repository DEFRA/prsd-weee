namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Domain;

    public class Partnership : Entity
    {
        public Partnership(string name, ProducerContact principalPlaceOfBusiness, List<Partner> partnersList)
        {
            PartnersList = partnersList;
            Name = name;
            PrincipalPlaceOfBusiness = principalPlaceOfBusiness;
        }

        protected Partnership()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var partnershipObj = obj as Partnership;
            if (partnershipObj == null)
            {
                return false;
            }
            return Name.Equals(partnershipObj.Name)
                   && PartnersList.Equals(partnershipObj.PartnersList)
                   && PrincipalPlaceOfBusiness != null &&
                   PrincipalPlaceOfBusiness.Equals(partnershipObj.PrincipalPlaceOfBusiness);
        }

        public string Name { get; private set; }

        public virtual Guid PrincipalPlaceOfBusinessId { get; private set; }

        public virtual ProducerContact PrincipalPlaceOfBusiness { get; private set; }

        public List<Partner> PartnersList { get; private set; }
    }
}
