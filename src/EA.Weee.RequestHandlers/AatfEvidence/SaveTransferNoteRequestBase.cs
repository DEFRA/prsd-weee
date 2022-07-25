namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.Helpers;
    using Domain.Organisation;
    using Domain.Scheme;

    public abstract class SaveTransferNoteRequestBase
    {
        public void ValidToSave(Organisation organisation, int complianceYear, DateTime systemDateTime)
        {
            if (!organisation.IsBalancingScheme)
            {
                if (organisation.Scheme.SchemeStatus == SchemeStatus.Withdrawn || !WindowHelper.IsDateInComplianceYear(complianceYear, systemDateTime))
                {
                    throw new InvalidOperationException("You cannot create transfer evidence as scheme is not in a valid state");
                }
            }
        }
    }
}
