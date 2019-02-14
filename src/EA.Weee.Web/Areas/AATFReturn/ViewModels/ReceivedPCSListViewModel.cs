namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Scheme;

    public class ReceivedPCSListViewModel : ReturnViewModelBase
    {
        public Guid ReturnId { get; set; }

        public Guid SchemeId { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public List<SchemeData> SchemeList { get; set; }

        public ReceivedPCSListViewModel()
        {
        }
    }
}