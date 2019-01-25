namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;          

    public partial class NonObligatedWeee : Entity
    {
        public NonObligatedWeee(Guid nonobligatedid, Guid returnid, Enum categoryid, bool dcf, decimal tonnage)
        {
            NonObligatedId = nonobligatedid;
            ReturnId = returnid;
            Dcf = dcf;
            Tonnage = tonnage;
        }

        public Guid NonObligatedId { get; private set; }

        public Guid ReturnId { get; private set; }

        public Enum CategoryId { get; private set; }

        public bool Dcf { get; private set; }

        public decimal Tonnage { get; private set; }
    }
}
