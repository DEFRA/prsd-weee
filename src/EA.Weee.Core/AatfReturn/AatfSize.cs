﻿namespace EA.Weee.Core.AatfReturn
{
    using Prsd.Core.Domain;

    public class AatfSize : Enumeration
    {
        public static readonly AatfSize Small = new AatfSize(1, "Small");
        public static readonly AatfSize Medium = new AatfSize(2, "Medium");
        public static readonly AatfSize Large = new AatfSize(3, "Large");
        protected AatfSize()
        {
        }

        public AatfSize(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
