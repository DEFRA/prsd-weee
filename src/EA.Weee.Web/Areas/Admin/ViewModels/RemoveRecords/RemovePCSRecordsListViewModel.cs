namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Admin;
    using Core.Shared.Paging;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Requests.Admin;

    public class RemovePCSRecordsListViewModel
    {
        public int? SelectedYear { get; set; }

        public string SelectedSchemeName { get; set; }

        public List<SchemeDataExceedingRetentionPeriod> SchemeData { get; set; }

        public RemovePCSRecordsListViewModel()
        {
        }
    }
}