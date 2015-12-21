namespace EA.Weee.Core.Search
{
    using System.Collections.Generic;

    /// <summary>
    ///  A base class for search results.
    ///  
    /// This class supports search algorithms which can pre-process search results
    /// and wish to store meta data against search results for performance gains.
    /// </summary>
    public abstract class SearchResult
    {
        private Dictionary<string, object> metadatas;
        private object metatdataLock = new object();

        public void AddMetadata<T>(string algorithm, T metadata) where T : class
        {
            EnsureDictionaryCreated();

            metadatas.Add(algorithm, metadata);
        }

        public T GetMetadata<T>(string algorithm) where T : class
        {
            EnsureDictionaryCreated();

            if (metadatas.ContainsKey(algorithm))
            {
                return metadatas[algorithm] as T;
            }
            else
            {
                return null;
            }
        }

        private void EnsureDictionaryCreated()
        {
            if (metadatas == null)
            {
                lock (metatdataLock)
                {
                    if (metadatas == null)
                    {
                        metadatas = new Dictionary<string, object>();
                    }
                }
            }
        }
    }
}
