namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;

    public class ObligatedProcess
    {
        public List<string> ObligatedProcessList { get; set; } = new List<string>() { "Received on behalf of PCS(s)", "Sent to another AATF / ATF", "Reused as a whole appliance" };
    }
}
