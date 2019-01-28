namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;          

    public partial class NonObligatedWeee : Entity
    {
        public NonObligatedWeee(Guid nonobligatedid, Return aatfReturn, Enum categoryid, bool dcf, decimal tonnage)
        {
            NonObligatedId = nonobligatedid;
            AatfReturn = aatfReturn;
            Dcf = dcf;
            Tonnage = tonnage;
        }

        public NonObligatedWeee()
        {
        }

        public Guid NonObligatedId { get; set; }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }

        public bool Dcf { get; set; }

        public decimal Tonnage { get; set; }

        public virtual Return AatfReturn { get; private set; }
    }
}
