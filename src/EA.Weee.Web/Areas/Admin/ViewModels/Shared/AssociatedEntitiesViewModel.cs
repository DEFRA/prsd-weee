namespace EA.Weee.Web.Areas.Admin.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.AatfReturn;

    public class AssociatedEntitiesViewModel
    {
        public List<AatfDataList> AssociatedAatfs { get; set; }

        public List<AatfDataList> AssociatedAes { get; set; }

        public List<Core.Scheme.SchemeData> AssociatedSchemes { get; set; }

        public bool HasAnyRelatedEntities => IsNotNullOrEmpty(AssociatedAatfs) || IsNotNullOrEmpty(AssociatedAes) || IsNotNullOrEmpty(AssociatedSchemes);

        private bool IsNotNullOrEmpty<T>(IList<T> entityList)
        {
            return entityList != null && entityList.Any();
        }
    }
}