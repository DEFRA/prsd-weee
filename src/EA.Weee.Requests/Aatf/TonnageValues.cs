namespace EA.Weee.Requests.Aatf
{
    using System;

    public class TonnageValues
    {
        public Guid Id { get; private set; }

        public int CategoryId { get; private set; }

        public decimal? FirstTonnage { get; private set; }

        public decimal? SecondTonnage { get; private set; }

        public TonnageValues(Guid id, int categoryId, decimal? firstTonnage, decimal? secondTonnage)
        {
            Id = id;
            CategoryId = categoryId;
            FirstTonnage = firstTonnage;
            SecondTonnage = secondTonnage;
        }
    }
}
