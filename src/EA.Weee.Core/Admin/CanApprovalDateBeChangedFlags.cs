namespace EA.Weee.Core.Admin
{
    using System;

    [Flags]
    public enum CanApprovalDateBeChangedFlags
    {
        DateChanged = 1,
        HasStartedReturn = 2,
        HasSubmittedReturn = 8,
        HasMultipleFacility = 16,
        HasResubmission = 32
    }
}
