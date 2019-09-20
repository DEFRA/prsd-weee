﻿namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;

    public class ReturnStatus : Enumeration
    {
        public static readonly ReturnStatus Created = new ReturnStatus(1, "Not Submitted");
        public static readonly ReturnStatus Submitted = new ReturnStatus(2, "Submitted");

        protected ReturnStatus()
        {
        }

        private ReturnStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
