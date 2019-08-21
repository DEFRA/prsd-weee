﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Submissions
{
    using Core.DataReturns;
    using System;
    using System.Collections.Generic;
    using Weee.Requests.Shared;

    public class DataReturnSubmissionsResultsViewModel
    {
        public int Year { get; set; }

        public Guid Scheme { get; set; }

        public DataReturnSubmissionsHistoryOrderBy OrderBy { get; set; }

        public IList<DataReturnSubmissionsHistoryData> Results { get; set; }
    }
}