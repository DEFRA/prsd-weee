﻿namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Core.Helpers;
    using Microsoft.Ajax.Utilities;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;

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
                    category.B2B = weeeObligatedData.B2B.ToTonnageEditDisplay();
                    category.B2C = weeeObligatedData.B2C.ToTonnageEditDisplay();
                    category.Id = weeeObligatedData.Id;
                }
            }

            return copyValues;
        }
    }
}