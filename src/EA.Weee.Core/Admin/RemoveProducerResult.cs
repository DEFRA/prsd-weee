namespace EA.Weee.Core.Admin
{
    public class RemoveProducerResult
    {
        public bool InvalidateProducerSearchCache { get; private set; }

        public RemoveProducerResult(bool invalidateProducerSearchCache)
        {
            InvalidateProducerSearchCache = invalidateProducerSearchCache;
        }
    }
}
