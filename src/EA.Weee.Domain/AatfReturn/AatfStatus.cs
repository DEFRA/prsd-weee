﻿namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;
    using Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AatfStatus : Enumeration
    {
        public static readonly AatfStatus Approved = new AatfStatus(2, "Approved");
        public static readonly AatfStatus Suspended = new AatfStatus(3, "Suspended");
        public static readonly AatfStatus Cancelled = new AatfStatus(4, "Cancelled");
        protected AatfStatus()
        {
        }

        private AatfStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
