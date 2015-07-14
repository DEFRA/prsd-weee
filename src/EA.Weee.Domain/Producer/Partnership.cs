namespace EA.Weee.Domain.Producer
{
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

        public string Name { get; private set; }

        public ProducerContact PrincipalPlaceOfBusiness { get; private set; }

        public List<Partner> PartnersList { get; private set; } 
    }
}
