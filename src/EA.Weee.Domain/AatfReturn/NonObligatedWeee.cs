﻿namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;          

    public partial class NonObligatedWeee : Entity, IReturnOption
    {
        public NonObligatedWeee(Return aatfReturn, int categoryId, bool dcf, decimal? tonnage)
        {
            Guard.ArgumentNotNull(() => aatfReturn, aatfReturn);

            Return = aatfReturn;
            Dcf = dcf;
            Tonnage = tonnage;
            CategoryId = categoryId;
        }

        public NonObligatedWeee()
        {
        }

        public virtual void UpdateTonnage(decimal? tonnage)
        {
            Tonnage = tonnage;
        }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }

        public bool Dcf { get; set; }

        public decimal? Tonnage { get; set; }

        public virtual Return Return { get; private set; }
    }
}
