namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;
    using System.Collections.Generic;

    public interface IPasteProcessor
    {
        PastedValues BuildModel(string pasteValues);

        IList<ObligatedCategoryValue> ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues, IList<ObligatedCategoryValue> existingData);

        IList<NonObligatedCategoryValue> ParseNonObligatedPastedValues(PastedValues nonObligatedPastedValues, IList<NonObligatedCategoryValue> existingData);
    }
}
