﻿namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AatfObligatedData
    {
        public AatfObligatedData(AatfData aatf, ObligatedCategoryValue weeeReceived, ObligatedCategoryValue weeeReused, ObligatedCategoryValue weeeSentOn)
        {
            Aatf = aatf;
            WeeeReceived = weeeReceived;
            WeeeReused = weeeReused;
            WeeeSentOn = weeeSentOn;
        }

        public AatfObligatedData(AatfData aatf)
        {
            Aatf = aatf;
        }
        public AatfObligatedData()
        {
        }
        
        public AatfData Aatf { get; set; }

        [Display(Name = "Received on behalf of PCS(s)")]
        public ObligatedCategoryValue WeeeReceived { get; set; } = new ObligatedCategoryValue("0.000", "0.000");

        [Display(Name = "Reused as a whole appliance")]
        public ObligatedCategoryValue WeeeReused { get; set; } = new ObligatedCategoryValue("0.000", "0.000");

        [Display(Name = "Sent to another AATF / ATF")]
        public ObligatedCategoryValue WeeeSentOn { get; set; } = new ObligatedCategoryValue("0.000", "0.000");
    }
}
