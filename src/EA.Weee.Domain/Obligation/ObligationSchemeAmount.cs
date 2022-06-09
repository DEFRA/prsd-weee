namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using CuttingEdge.Conditions;
    using Lookup;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;

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
    }
}
