namespace EA.Weee.Core.AatfReturn
{
    using System.ComponentModel.DataAnnotations;

    public class AatfObligatedData
    {
        public AatfObligatedData(Aatf aatf, ObligatedCategoryValue weeeReceived, ObligatedCategoryValue weeeReused, ObligatedCategoryValue weeeSentOn)
        {
            Aatf = aatf;
            WeeeReceived = weeeReceived;
            WeeeReused = weeeReused;
            WeeeSentOn = weeeSentOn;
        }

        public AatfObligatedData(Aatf aatf)
        {
            Aatf = aatf;
        }
        public AatfObligatedData()
        {
        }
        
        public Aatf Aatf { get; set; }

        [Display(Name = "Received on behalf of PCS(s)")]
        public ObligatedCategoryValue WeeeReceived { get; set; } = new ObligatedCategoryValue("-", "-");

        [Display(Name = "Reused as a whole appliance")]
        public ObligatedCategoryValue WeeeReused { get; set; } = new ObligatedCategoryValue("-", "-");

        [Display(Name = "Sent to another AATF / ATF")]
        public ObligatedCategoryValue WeeeSentOn { get; set; } = new ObligatedCategoryValue("-", "-");
    }
}
