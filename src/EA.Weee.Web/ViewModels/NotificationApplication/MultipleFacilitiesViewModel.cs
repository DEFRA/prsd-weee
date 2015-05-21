namespace EA.Weee.Web.ViewModels.NotificationApplication
{
    using System;
    using System.Collections.Generic;
    using Requests.Facilities;

    public class MultipleFacilitiesViewModel
    {
        public MultipleFacilitiesViewModel()
        {
            FacilityData = new List<FacilityData>();
        }

        public Guid NotificationId { get; set; }

        public List<FacilityData> FacilityData { get; set; }
    }
}