﻿namespace EA.Weee.Core.AatfReturn
{
    using System;
    using DataReturns;
    using Validation;

    [Serializable]
    public class NonObligatedCategoryValue : CategoryValue
    {
        [TonnageValue("CategoryId")]
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
