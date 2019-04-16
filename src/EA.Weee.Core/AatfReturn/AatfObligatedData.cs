namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Scheme;
    using EA.Prsd.Core;

    public class AatfObligatedData
    {
        public AatfObligatedData(AatfData aatf, List<AatfSchemeData> schemeData)
        {
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => schemeData, schemeData);

            Aatf = aatf;
            SchemeData = schemeData;
        }
        public AatfObligatedData()
        {
        }
        
        public AatfData Aatf { get; set; }

        public List<AatfSchemeData> SchemeData { get; set; }

        [Display(Name = "Received on behalf of PCS(s)")]
        public ObligatedCategoryValue WeeeReceived { get; set; } = new ObligatedCategoryValue("-", "-");

        [Display(Name = "Reused as a whole appliance")]
        public ObligatedCategoryValue WeeeReused { get; set; } = new ObligatedCategoryValue("-", "-");

        [Display(Name = "Sent to another AATF / ATF")]
        public ObligatedCategoryValue WeeeSentOn { get; set; } = new ObligatedCategoryValue("-", "-");
    }
}
