namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using Microsoft.Ajax.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class NonObligatedDataToNonObligatedValueMap : IMap<NonObligatedDataToNonObligatedValueMapTransfer, IList<NonObligatedCategoryValue>>
    {
        public IList<NonObligatedCategoryValue> Map(NonObligatedDataToNonObligatedValueMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.NonObligatedCategoryValues, source.NonObligatedCategoryValues);

            var nonObligatedList = new List<NonObligatedCategoryValue>();
            source.NonObligatedCategoryValues.CopyItemsTo(nonObligatedList);

            foreach (var weeeNonObligatedData in source.NonObligatedDataValues)
            {
                var category = nonObligatedList.FirstOrDefault(c => c.CategoryId == weeeNonObligatedData.CategoryId);
                if (source.Dcf == weeeNonObligatedData.Dcf)
                {
                    if (category != null)
                    {
                        category.Tonnage = weeeNonObligatedData.Tonnage.ToString();
                        category.Dcf = weeeNonObligatedData.Dcf;
                        category.Id = weeeNonObligatedData.Id;
                    }
                }
            }

            return nonObligatedList;
        }
    }
}