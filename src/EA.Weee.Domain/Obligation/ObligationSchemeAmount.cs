namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Lookup;
    using Prsd.Core.Domain;

    public partial class ObligationSchemeAmount : Entity
    {
        [ForeignKey("ObligationSchemeId")]
        public virtual ObligationScheme ObligationScheme { get; private set; }

        public virtual Guid ObligationSchemeId { get; private set; }

        public virtual WeeeCategory CategoryId { get; private set; }

        public virtual decimal? Obligation { get; private set; }

        public ObligationSchemeAmount()
        {
        }

        public ObligationSchemeAmount(WeeeCategory categoryId, decimal? obligation)
        {
            CategoryId = categoryId;
            Obligation = obligation;
        }

        public virtual bool UpdateObligation(decimal? updatedAmount)
        {
            var changed = false;
            if (!updatedAmount.HasValue && Obligation.HasValue)
            {
                changed = true;
            }
            else if (!Obligation.HasValue && updatedAmount.HasValue)
            {
                changed = true;
            }
            else if (decimal.Compare(updatedAmount.GetValueOrDefault(), Obligation.GetValueOrDefault()) != 0)
            {
                changed = true;
            }

            if (changed)
            {
                Obligation = updatedAmount;
            }

            return changed;
        }
    }
}
