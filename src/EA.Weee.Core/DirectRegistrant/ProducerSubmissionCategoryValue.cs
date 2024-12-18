namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;

    public class ProducerSubmissionCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The household tonnage value", true)]
        public virtual string HouseHold { get; set; }

        [TonnageValue(nameof(CategoryId), "The non-household tonnage value", true)]
        public virtual string NonHouseHold { get; set; }

        public ProducerSubmissionCategoryValue()
        {
        }

        public ProducerSubmissionCategoryValue(string houseHold, string nonHouseHold)
        {
            HouseHold = houseHold;
            NonHouseHold = nonHouseHold;
        }

        public ProducerSubmissionCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}
