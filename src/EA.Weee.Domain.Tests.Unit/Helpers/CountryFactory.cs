namespace EA.Weee.Domain.Tests.Unit.Helpers
{
    using Domain;
    using System;
 
    public class CountryFactory
    {
        public static Country Create(Guid id, string name = "test", bool isEuMember = true)
        {
            var country = ObjectInstantiator<Country>.CreateNew();

            ObjectInstantiator<Country>.SetProperty(x => x.Id, id, country);
            ObjectInstantiator<Country>.SetProperty(x => x.Name, name, country);  
            return country;
        }
    }
}
