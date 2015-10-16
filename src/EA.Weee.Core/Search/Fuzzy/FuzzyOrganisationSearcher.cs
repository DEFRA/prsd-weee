namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A fuzzy searcher that searches organisations.
    /// </summary>
    public class FuzzyOrganisationSearcher : FuzzySearcher<OrganisationSearchResult>
    {
        public FuzzyOrganisationSearcher(ISearchResultProvider<OrganisationSearchResult> searchResultProvider)
            : base(searchResultProvider)
        {
        }

        protected override void DefineSynonymsAndCommonTerms()
        {
            Synonym limited = new Synonym(new List<string>() { "limited", "ltd", "ltd." });
            Synonyms.Add(limited);

            Synonym company = new Synonym(new List<string>() { "company", "co", "co." });
            Synonyms.Add(company);

            CommonTerms.Add(new CommonTerm(limited, 0.25));
            CommonTerms.Add(new CommonTerm(company, 0.5));
            CommonTerms.Add(new CommonTerm(new Word("plc"), 0.5));
            CommonTerms.Add(new CommonTerm(new Word("compliance"), 0.5));
            CommonTerms.Add(new CommonTerm(new Word("weee"), 0.5));
            CommonTerms.Add(new CommonTerm(new Word("scheme"), 0.5));
            CommonTerms.Add(new CommonTerm(new Word("pcs"), 0.5));
        }

        public override IEnumerable<string> Split(OrganisationSearchResult result)
        {
            return Split(result.Name);
        }
        
        public override IEnumerable<string> Split(string phrase)
        {
            IEnumerable<string> words = phrase
                .ToLowerInvariant()
                .Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Names which start with WEEE such as "WEEECompany" will be split into
            // separate words "weee" and "company"
            foreach (string word in words)
            {
                if (word.StartsWith("weee") && word != "weee")
                {
                    yield return "weee";
                    yield return word.Substring(4);
                }
                else
                {
                    yield return word;
                }
            }
        }

        protected override double ConfidenceThreshold
        {
            get { return 0.7; }
        }
    }
}
