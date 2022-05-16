namespace EA.Weee.Requests.Aatf
{
    using System;

    public class TonnageValues
    {
        public virtual Guid Id { get; private set; }

        public virtual int CategoryId { get; private set; }

        public virtual decimal? FirstTonnage { get; private set; }

        public virtual decimal? SecondTonnage { get; private set; }

        public TonnageValues(Guid id, int categoryId, decimal? firstTonnage, decimal? secondTonnage)
        {
            Id = id;
            CategoryId = categoryId;
            FirstTonnage = firstTonnage;
            SecondTonnage = secondTonnage;
        }
    }
}
