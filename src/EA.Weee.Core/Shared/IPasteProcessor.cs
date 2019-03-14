namespace EA.Weee.Core.Shared
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public interface IPasteProcessor
    {
        PastedValues BuildModel(string pasteValues);

        IList<ObligatedCategoryValue> ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues, IList<ObligatedCategoryValue> existingData);
    }
}
