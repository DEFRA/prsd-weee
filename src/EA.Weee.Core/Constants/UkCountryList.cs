namespace EA.Weee.Core.Constants
{
    using System;
    using System.Collections.Generic;

    public static class UkCountryList
    {
        public static readonly HashSet<Guid> ValidCountryIds = new HashSet<Guid>
        {
            Guid.Parse("DB83F5AB-E745-49CF-B2CA-23FE391B67A8"),
            Guid.Parse("4209EE95-0882-42F2-9A5D-355B4D89EF30"),
            Guid.Parse("184E1785-26B4-4AE4-80D3-AE319B103ACB"),
            Guid.Parse("7BFB8717-4226-40F3-BC51-B16FDF42550C")
        };
    }
}
