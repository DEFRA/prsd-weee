namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Domain;
    using Prsd.Core.Domain;

    public class Partnership : Entity, IEquatable<Partnership>
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

        public virtual bool Equals(Partnership other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name &&
                   object.Equals(PrincipalPlaceOfBusiness, other.PrincipalPlaceOfBusiness) &&
                   PartnersList.ElementsEqual(other.PartnersList);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Partnership);
        }

        public virtual string Name { get; private set; }

        public virtual Guid PrincipalPlaceOfBusinessId { get; private set; }

        public virtual ProducerContact PrincipalPlaceOfBusiness { get; private set; }

        public virtual List<Partner> PartnersList { get; private set; }
    }
}
