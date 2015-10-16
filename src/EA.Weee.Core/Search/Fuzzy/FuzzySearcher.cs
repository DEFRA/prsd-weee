namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A searcher that uses the Levenshtein distance to apply fuzzy searching logic
    /// on a per-word basis.
    /// 
    /// Contextual intelligence is provided by defining a list of synonyms, a list
    /// of common terms and by inteliigent term-splitting.
    /// 
    /// Results are returned in order of relevance.
    /// 
    /// Due to the computationally intensive nature of the search, this searcher should
    /// only be used to query result sets for as-you-type searching where the number of
    /// items is small (no more than 1,000 items).
    /// </summary>
    /// <typeparam name="T">The type of search result.</typeparam>
    public abstract class FuzzySearcher<T> : ISearcher<T> where T : SearchResult
    {
        private const string Algorithm = "fuzzy";
        private readonly ISearchResultProvider<T> searchResultProvider;
        
        protected readonly List<Synonym> Synonyms = new List<Synonym>();
        protected readonly List<CommonTerm> CommonTerms = new List<CommonTerm>();

        private bool initialized;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public FuzzySearcher(ISearchResultProvider<T> searchResultProvider)
        {
            this.searchResultProvider = searchResultProvider;
        }

        private async Task<IList<T>> FetchResults()
        {
            IList<T> results = await searchResultProvider.FetchAll();
            foreach (T result in results)
            {
                if (result.GetMetadata<ResultMetadata>(Algorithm) == null)
                {
                    ResultMetadata resultMetadata = ParseResult(result);
                    result.AddMetadata(Algorithm, resultMetadata);
                }
            }
            return results;
        }

        private async Task EnsureInitialized()
        {
            if (!initialized)
            {
                await semaphore.WaitAsync();

                if (!initialized)
                {
                    DefineSynonymsAndCommonTerms();
                    initialized = true;
                }

                semaphore.Release();
            }
        }

        public ResultMetadata ParseResult(T result)
        {
            List<ResultTerm> resultTerms = new List<ResultTerm>();

            IEnumerable<string> words = Split(result);
            
            foreach (string word in words)
            {
                ITerm term = null;
                double relevance = 1;

                foreach (Synonym synonym in Synonyms)
                {
                    if (synonym.Values.Any(v => v == word))
                    {
                        term = synonym;
                        break;
                    }
                }

                if (term != null)
                {
                    foreach (CommonTerm commonTerm in CommonTerms)
                    {
                        if (commonTerm.Term == term)
                        {
                            relevance = commonTerm.Relevance;
                            break;
                        }
                    }
                }
                else
                {
                    term = new Word(word);
                }
                
                ResultTerm resultTerm = new ResultTerm(term, relevance);
                resultTerms.Add(resultTerm);
            }

            return new ResultMetadata(resultTerms);
        }

        public IList<SearchWord> ParseSearchPhrase(string searchPhrase, bool asYouType)
        {
            List<SearchWord> searchWords = new List<SearchWord>();

            List<string> words = Split(searchPhrase).ToList();

            for (int index = 0; index < words.Count; ++index)
            {
                string value = words[index];
                bool isPartial = asYouType && (index == words.Count - 1) && (searchPhrase == searchPhrase.TrimEnd());

                SearchWord searchWord = new SearchWord(value, isPartial);
                searchWords.Add(searchWord);
            }

            return searchWords;
        }

        public Rank Match(IList<SearchWord> searchPhrase, IList<ResultTerm> resultTerms)
        {
            double[,] ranks = new double[searchPhrase.Count, resultTerms.Count];

            for (int searchRank = 0; searchRank < searchPhrase.Count; ++searchRank)
            {
                for (int resultRank = 0; resultRank < resultTerms.Count; ++resultRank)
                {
                    ranks[searchRank, resultRank] = Match(searchPhrase[searchRank], resultTerms[resultRank]);
                }
            }

            double relevance = 0;
            double confidence = 0;
            List<int> matchedSearchWordIndexes = new List<int>();
            List<int> matchedResultTermIndexes = new List<int>();
            for (int index = 0; index < Math.Min(searchPhrase.Count, resultTerms.Count); ++index)
            {
                double bestMatchSoFar = 0;
                int? bestSearchRank = null;
                int? bestResultRank = null;

                for (int searchRank = 0; searchRank < searchPhrase.Count; ++searchRank)
                {
                    if (matchedSearchWordIndexes.Contains(searchRank))
                    {
                        continue;
                    }

                    for (int resultRank = 0; resultRank < resultTerms.Count; ++resultRank)
                    {
                        if (matchedResultTermIndexes.Contains(resultRank))
                        {
                            continue;
                        }

                        double match = ranks[searchRank, resultRank];
                        if (match >= bestMatchSoFar)
                        {
                            bestMatchSoFar = match;
                            bestSearchRank = searchRank;
                            bestResultRank = resultRank;
                        }
                    }
                }

                matchedSearchWordIndexes.Add(bestSearchRank.Value);
                matchedResultTermIndexes.Add(bestResultRank.Value);
                relevance += bestMatchSoFar * resultTerms[bestResultRank.Value].Relevance;
                confidence += bestMatchSoFar;
            }

            confidence /= searchPhrase.Count;

            return new Rank(confidence, relevance);
        }

        public double Match(SearchWord searchWord, ResultTerm resultTerm)
        {
            double bestConfidenceSoFar = 0;

            foreach (string resultValue in resultTerm.Term.Values)
            {
                double weightedDistance;
                if (!searchWord.IsPartial || searchWord.Value.Length >= resultValue.Length)
                {
                    int distance = LevenshteinDistance.Compute(searchWord.Value, resultValue);
                    
                    weightedDistance = (double)distance / (double)resultValue.Length;
                }
                else
                {
                    string partialValue = resultValue.Substring(0, searchWord.Value.Length);

                    int distance = LevenshteinDistance.Compute(searchWord.Value, partialValue);

                    weightedDistance = ((double)distance / (double)partialValue.Length);
                }

                double confidence = 1 / (1 + weightedDistance);

                if (confidence > bestConfidenceSoFar)
                {
                    bestConfidenceSoFar = confidence;
                }
            }

            return bestConfidenceSoFar;
        }

        public async Task<IList<T>> Search(string searchTerm, int maxResults, bool asYouType)
        {
            await EnsureInitialized();

            IList<SearchWord> searchPhrase = ParseSearchPhrase(searchTerm, asYouType);

            var ranks = new Dictionary<T, Rank>();

            foreach (T result in await FetchResults())
            {
                ResultMetadata resultMetadata = result.GetMetadata<ResultMetadata>(Algorithm);
                Rank rank = Match(searchPhrase, resultMetadata.ResultTerms);
                ranks.Add(result, rank);
            }

            return ranks
                .Where(r => r.Value.Confidence >= ConfidenceThreshold)
                .OrderByDescending(r => r.Value.Relevance)
                .Take(maxResults)
                .Select(r => r.Key)
                .ToList();
        }

        protected virtual void DefineSynonymsAndCommonTerms()
        {
        }

        public abstract IEnumerable<string> Split(T result);

        public abstract IEnumerable<string> Split(string phrase);

        /// <summary>
        /// A value between 0 and 1 which determines how confidence the match must be
        /// for a result to be returned.
        /// A threshold of 0 will return all results.
        /// A threshold of 1 will reutrn only exact matches with no common terms.
        /// </summary>
        protected abstract double ConfidenceThreshold { get; }
    }
}
