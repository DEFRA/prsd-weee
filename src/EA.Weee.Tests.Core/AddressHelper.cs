namespace EA.Weee.Tests.Core
{
    using Domain.AatfReturn;
    using Model;
    using System.Linq;

    public static class AddressHelper
    {
        public static AatfAddress GetAatfAddress(DatabaseWrapper database)
        {
            return new AatfAddress("name", "address", "address2", "town", "county", "postcode", database.WeeeContext.Countries.First());
        }
    }
}
