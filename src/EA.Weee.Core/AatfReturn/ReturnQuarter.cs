namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;

    public class ReturnQuarter
    {
        public int ComplianceYear { get; private set; }

        public QuarterType Quarter { get; private set; }

        public ReturnQuarter(int complianceYear, QuarterType quarter)
        {
            ComplianceYear = complianceYear;
            Quarter = quarter;
        }
    }
}
