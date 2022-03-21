namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.AatfReturn;

    public class SelectYourAatfViewModel
    {
        public Guid OrganisationId { get; set; }

        public FacilityType FacilityType { get; set; }

        [DisplayName("Which site would you like to manage evidence notes for?")]
        public Guid? SelectedId { get; set; }

        public IReadOnlyList<AatfData> AatfList { get; set; }

        public bool ModelValidated { get; private set; }

        public SelectYourAatfViewModel() 
        { 
        }
    }
}