namespace EA.Weee.Core.AatfReturn
{
    using System;
    using DataReturns;
    using Validation;

    [Serializable]
    public class NonObligatedCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The tonnage value")]
        public string Tonnage { get; set; }

        public bool Dcf { get; set; }

        public NonObligatedCategoryValue()
        {
        }

        public NonObligatedCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}
