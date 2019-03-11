namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;

    public interface IPasteProcessor
    {
        ObligatedCategoryValues BuildModel(object pasteValues);
    }
}
