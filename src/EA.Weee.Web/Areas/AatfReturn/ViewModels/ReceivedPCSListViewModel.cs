﻿namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;
    using System;
    using System.Collections.Generic;

    public class ReceivedPcsListViewModel : ReturnViewModelBase
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public List<ReceivedPcsData> SchemeList { get; set; }

        public ReceivedPcsListViewModel()
        {
        }
    }
}