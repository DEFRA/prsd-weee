namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.Helpers;
    using Microsoft.Ajax.Utilities;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ObligatedDataToObligatedValueMap : IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>>
    {
        public IList<ObligatedCategoryValue> Map(ObligatedDataToObligatedValueMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.ObligatedCategoryValues, source.ObligatedCategoryValues);
            Guard.ArgumentNotNull(() => source.WeeeDataValues, source.WeeeDataValues);

            var copyValues = new List<ObligatedCategoryValue>();
            source.ObligatedCategoryValues.CopyItemsTo(copyValues);

            foreach (var weeeObligatedData in source.WeeeDataValues)
            {
                var category = copyValues.FirstOrDefault(c => c.CategoryId == weeeObligatedData.CategoryId);

                if (category != null)
                {
                    category.B2B = weeeObligatedData.B2B.ToTonnageDisplay(true);
                    category.B2C = weeeObligatedData.B2C.ToTonnageDisplay(true);
                    category.Id = weeeObligatedData.Id;
                }
            }

            return copyValues;
        }
    }
}