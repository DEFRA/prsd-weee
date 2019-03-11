namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    
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
            if (tonnage == null)
            {
                tonnage = 0.000m;
            }

            return tonnage;
        }

        public string CheckIfTonnageIsNull(decimal? tonnage)
        {
            return (tonnage != null) ? tonnage.ToTonnageDisplay() : "-";
        }
    }
}