namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.Helpers;
    using Domain.AatfReturn;

    public abstract class SaveEvidenceNoteRequestBase
    {
        public void AatfIsValidToSave(Aatf aatf, DateTime systemDateTime)
        {
            if (aatf == null || 
                aatf.AatfStatus != AatfStatus.Approved 
                || !WindowHelper.IsDateInComplianceYear(aatf.ComplianceYear, systemDateTime)
                || (aatf.ApprovalDate.HasValue && aatf.ApprovalDate.Value.Date > systemDateTime.Date))
            {
                throw new InvalidOperationException("You cannot create evidence if the start and end dates are not in the current compliance year");
            }
        }
    }
}
