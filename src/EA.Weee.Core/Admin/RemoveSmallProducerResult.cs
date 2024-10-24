namespace EA.Weee.Core.Admin
{
    public class RemoveSmallProducerResult
    {
        public bool InvalidateProducerSearchCache { get; private set; }

        public RemoveSmallProducerResult(bool invalidateProducerSearchCache)
        {
            InvalidateProducerSearchCache = invalidateProducerSearchCache;
        }
    }
}
