﻿namespace EA.Weee.Core.AatfReturn
{
    using System;
    using DataReturns;
    using EA.Weee.Core.Shared;
    using Validation;

    [Serializable]
    public class NonObligatedCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The tonnage value", false)]
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
