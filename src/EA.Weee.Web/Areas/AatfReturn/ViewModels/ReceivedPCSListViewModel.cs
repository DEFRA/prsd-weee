namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Scheme;

    public class ReceivedPcsListViewModel : ReturnViewModelBase
    {
        public Guid ReturnId { get; set; }
        
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public List<SchemeData> SchemeList { get; set; }

        public ReceivedPcsListViewModel()
        {
        }
    }
}