namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;

    public interface IPasteProcessor
    {
        ObligatedCategoryValues BuildModel(ObligatedCategoryValue pasteValues);
    }
}
