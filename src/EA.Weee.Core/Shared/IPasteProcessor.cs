namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;

    public interface IPasteProcessor
    {
        PastedValues BuildModel(string pasteValues);

        ObligatedCategoryValues ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues);
    }
}
