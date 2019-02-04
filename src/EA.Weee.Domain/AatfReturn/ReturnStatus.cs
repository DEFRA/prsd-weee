namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using User;

    public class ReturnStatus : Enumeration
    {
        public static readonly ReturnStatus Created = new ReturnStatus(1, "Created");
        public static readonly ReturnStatus Submitted = new ReturnStatus(2, "Submitted");

        protected ReturnStatus()
        {
        }

        private ReturnStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
