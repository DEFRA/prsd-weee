namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.AatfReturn;

    public class AssociatedEntitiesViewModelTransfer
    {
        public List<AatfDataList> AssociatedAatfs { get; set; }

        public List<Core.Scheme.SchemeData> AssociatedSchemes { get; set; }

        public Guid? AatfId { get; set; }

        public Guid? SchemeId { get; set; }
    }
}