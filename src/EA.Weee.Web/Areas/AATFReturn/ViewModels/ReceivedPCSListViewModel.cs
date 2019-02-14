namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.AatfReturn;
    using Core.DataReturns;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReceivedPCSListViewModel : ReturnViewModelBase
    {
        public Guid ReturnId { get; set; }

        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }
        
        public ReceivedPCSListViewModel()
        {
        }
    }
}