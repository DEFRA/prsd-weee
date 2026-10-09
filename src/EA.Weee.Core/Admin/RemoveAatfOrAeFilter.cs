namespace EA.Weee.Core.Admin
{
    using EA.Weee.Core.AatfReturn;

    public class RemoveAatfOrAeFilter
    {
        public RemoveAatfOrAeFilter()
        {
        }

        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public int? ComplianceYear { get; set; }

        public int? SelectedStatus { get; set; }

        public string UserId { get; set; }

        public FacilityType FacilityType { get; set; }

        public int RetenctionPeriod { get; set; }
    }
}
