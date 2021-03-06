﻿namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class SelectYourPcsViewModel
    {
        public SelectYourPcsViewModel(List<SchemeData> schemeList, List<Guid> selectedSchemes)
        {
            Guard.ArgumentNotNull(() => schemeList, schemeList);
            Guard.ArgumentNotNull(() => selectedSchemes, selectedSchemes);

            this.SchemeList = schemeList;
            this.SelectedSchemes = selectedSchemes;
        }

        public SelectYourPcsViewModel()
        {
        }

        public List<SchemeData> SchemeList { get; set; }

        [MinimumElements(1, "You must select at least one PCS from the list")]
        public List<Guid> SelectedSchemes { get; set; }

        public List<Guid> RemovedSchemes { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public bool Reselect { get; set; }

        public PreviousQuarterReturnResult PreviousQuarterData { get; set; }

        public bool CopyPrevious { get; set; }
    }
}