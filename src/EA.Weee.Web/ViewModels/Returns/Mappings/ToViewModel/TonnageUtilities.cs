﻿namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Core.Helpers;
    using System.Collections.Generic;

    public class TonnageUtilities : ITonnageUtilities
    {
        public ObligatedCategoryValue SumObligatedValues(List<WeeeObligatedData> dataSet)
        {
            decimal? b2bTotal = null;
            decimal? b2cTotal = null;

            if (dataSet.Count != 0)
            {
                foreach (var category in dataSet)
                {
                    if (category.B2B != null)
                    {
                        b2bTotal = InitialiseTotalDecimal(b2bTotal);
                        b2bTotal += category.B2B;
                    }
                    if (category.B2C != null)
                    {
                        b2cTotal = InitialiseTotalDecimal(b2cTotal);
                        b2cTotal += category.B2C;
                    }
                }
            }

            return new ObligatedCategoryValue(CheckIfTonnageIsNull(b2bTotal), CheckIfTonnageIsNull(b2cTotal));
        }

        public decimal? InitialiseTotalDecimal(decimal? tonnage)
        {
            return tonnage ?? (0.000m);
        }

        public string CheckIfTonnageIsNull(decimal? tonnage)
        {
            return (tonnage != null) ? tonnage.ToTonnageDisplay() : "-";
        }

        public string SumTotals(List<decimal?> values)
        {
            decimal? total = null;

            foreach (var @decimal in values)
            {
                if (@decimal.HasValue)
                {
                    if (!total.HasValue)
                    {
                        total = 0;
                    }

                    total += @decimal.Value;
                }
            }

            return CheckIfTonnageIsNull(total);
        }
    }
}