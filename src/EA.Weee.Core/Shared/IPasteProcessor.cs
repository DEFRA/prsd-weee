namespace EA.Weee.Core.Shared
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public interface IPasteProcessor
    {
        PastedValues BuildModel(string pasteValues);

        ObligatedCategoryValues ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues, IList<ObligatedCategoryValue> existingData);
    }
}
