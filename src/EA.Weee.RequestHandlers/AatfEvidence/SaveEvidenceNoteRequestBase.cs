namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.Helpers;
    using Domain.AatfReturn;

    public abstract class SaveEvidenceNoteRequestBase
    {
        public void AatfIsValidToSave(Aatf aatf, DateTime systemDateTime)
        {
            if (aatf.AatfStatus != AatfStatus.Approved || !WindowHelper.IsDateInComplianceYear(aatf.ComplianceYear, systemDateTime) 
                                                       || (aatf.ApprovalDate.HasValue && aatf.ApprovalDate.Value.Date > systemDateTime.Date))
            {
                throw new InvalidOperationException("You cannot create evidence if your site approval has been cancelled or suspended or your site is not approved for the selected compliance year");
            }
        }
    }
}
