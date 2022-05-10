namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;

    public interface IPasteProcessor
    {
        PastedValues BuildModel(string pasteValues);

        IList<ObligatedCategoryValue> ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues, IList<ObligatedCategoryValue> existingData);

        IList<NonObligatedCategoryValue> ParseNonObligatedPastedValues(PastedValues nonObligatedPastedValues, IList<NonObligatedCategoryValue> existingData);

        IList<EvidenceCategoryValue> ParseEvidencePastedValues(EvidencePastedValues evidencePastedValues, IList<EvidenceCategoryValue> existingData);
    }
}
