namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.Helpers;
    using Domain.Organisation;
    using Domain.Scheme;

    public abstract class SaveTransferNoteRequestBase
    {
        private static string YouCannotCreateTransferEvidenceAsSchemeIsNotInAValidState => "You cannot manage evidence as scheme is not in a valid state";

        public void ValidToSave(Organisation organisation, int complianceYear, DateTime systemDateTime)
        {
            if (!organisation.IsBalancingScheme)
            {
                if (organisation.Scheme.SchemeStatus == SchemeStatus.Withdrawn)
                {
                    throw new InvalidOperationException(YouCannotCreateTransferEvidenceAsSchemeIsNotInAValidState);
                }
            }

            if (!WindowHelper.IsDateInComplianceYear(complianceYear, systemDateTime))
            {
                throw new InvalidOperationException(YouCannotCreateTransferEvidenceAsSchemeIsNotInAValidState);
            }
        }
    }
}
