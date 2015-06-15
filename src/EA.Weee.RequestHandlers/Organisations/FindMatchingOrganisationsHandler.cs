namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Requests.Shared;
    using OrganisationType = EA.Weee.Domain.OrganisationType;

    internal class FindMatchingOrganisationsHandler :
        IRequestHandler<FindMatchingOrganisations, IList<OrganisationSearchData>>
    {
        private readonly WeeeContext context;

        private readonly SpecialBusinessSearchCase[] specialCases =
        {
            new SpecialBusinessSearchCase(new[] { "LTD", "LIMITED" }, Position.End),
            new SpecialBusinessSearchCase(new[] { "THE" }, Position.Start),
            new SpecialBusinessSearchCase(new[] { "PLC" }, Position.End),
            new SpecialBusinessSearchCase(new[] { "COMPANY", "CO" }, Position.End)
        };

        public FindMatchingOrganisationsHandler(WeeeContext context)
        {
            this.context = context;
        }

        private class OrganisationDataFields
        {
            private readonly Guid id;
            private readonly List<string> dataFields;

            public OrganisationDataFields(Guid id)
            {
                this.id = id;
                dataFields = new List<string>();
            }

            public Guid GetId()
            {
                return id;
            }

            public void AddDataField(string testData)
            {
                dataFields.Add(testData);
            }

            public IEnumerable<string> GetDataFields()
            {
                return dataFields.AsReadOnly();
            }
        }

        private IEnumerable<Func<Organisation, string>> GetDataExtractors()
        {
            Func<Organisation, string> getName = ((o) => (o.Name != null ? o.Name.ToUpperInvariant() : string.Empty));
            Func<Organisation, string> getTradingName = ((o) => (o.TradingName != null ? o.TradingName.ToUpperInvariant() : string.Empty));

            return new List<Func<Organisation, string>> { getName, getTradingName };
        }

        public async Task<IList<OrganisationSearchData>> HandleAsync(FindMatchingOrganisations query)
        {
            var searchTerm = PrepareQuery(query);

            // This search uses the Levenshtein edit distance as a search algorithm.
            var permittedDistance = CalculateMaximumLevenshteinDistance(searchTerm);

            var possibleOrganisations = await GetPossibleOrganisationNames(searchTerm);

            // extract data fields we want to compare against query and clean them up

            IEnumerable<Func<Organisation, string>> dataExtractors = GetDataExtractors();
            List<OrganisationDataFields> organisationDataFieldsCollection = new List<OrganisationDataFields>();

            foreach (var possibleOrganisation in possibleOrganisations)
            {
                var organisationDataFields = new OrganisationDataFields(possibleOrganisation.Id);

                foreach (var dataExtractor in dataExtractors)
                {
                    var dataField = dataExtractor(possibleOrganisation);

                    foreach (var specialCase in specialCases)
                    {
                        specialCase.CleanseSpecialCases(ref dataField);
                    }

                    organisationDataFields.AddDataField(dataField);
                }

                organisationDataFieldsCollection.Add(organisationDataFields);
            }

            // compare extracted data fields against query

            var matchingIdsWithDistance = new List<KeyValuePair<Guid, int>>();

            foreach (var organisationDataFields in organisationDataFieldsCollection)
            {
                var lowestDistance = int.MaxValue;

                foreach (string dataField in organisationDataFields.GetDataFields())
                {
                    var distance = StringSearch.CalculateLevenshteinDistance(searchTerm, dataField);
                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                    }
                }

                if (lowestDistance <= permittedDistance)
                {
                    matchingIdsWithDistance.Add(new KeyValuePair<Guid, int>(organisationDataFields.GetId(), lowestDistance));
                }
            }

            matchingIdsWithDistance = matchingIdsWithDistance.OrderBy(m => m.Value).ToList();

            var matchingOrganisations =
                matchingIdsWithDistance.Select(
                    m => possibleOrganisations.Single(o => o.Id == m.Key));

            return matchingOrganisations.Select(o => 
                new OrganisationSearchData
                {
                    Id = o.Id, 
                    Address = new AddressData
                    {
                        Address1 = o.OrganisationAddress.Address1,
                        Address2 = o.OrganisationAddress.Address2,
                        TownOrCity = o.OrganisationAddress.TownOrCity,
                        CountyOrRegion = o.OrganisationAddress.CountyOrRegion,
                        Postcode = o.OrganisationAddress.Postcode,
                        Country = o.OrganisationAddress.Country,
                        Telephone = o.OrganisationAddress.Telephone,
                        Email = o.OrganisationAddress.Email
                    }, 
                    DisplayName = o.OrganisationType == OrganisationType.RegisteredCompany ? o.Name : o.TradingName
                }).ToList();
        }

        private string PrepareQuery(FindMatchingOrganisations query)
        {
            var returnString = query.CompanyName.Trim().ToUpperInvariant();

            foreach (var specialCase in specialCases)
            {
                specialCase.CleanseSpecialCases(ref returnString);
            }

            return returnString;
        }

        private int CalculateMaximumLevenshteinDistance(string searchTerm)
        {
            var distance = 1;

            if (searchTerm.Length >= 5 && searchTerm.Length < 9)
            {
                distance = 2;
            }
            else if (searchTerm.Length >= 9 && searchTerm.Length < 15)
            {
                distance = 3;
            }
            else if (searchTerm.Length >= 15)
            {
                distance = 4;
            }

            return distance;
        }

        private async Task<Organisation[]> GetPossibleOrganisationNames(string searchTerm)
        {
            var firstLetterOfSearchTerm = searchTerm[0].ToString();

            return await context.Organisations
                .Where(o => (o.Name != null && o.Name.StartsWith(firstLetterOfSearchTerm))
                         || (o.Name != null && o.Name.StartsWith("THE "))
                         || (o.TradingName != null && o.TradingName.StartsWith(firstLetterOfSearchTerm))
                         || (o.TradingName != null && o.TradingName.StartsWith("THE ")))
                .ToArrayAsync();
        }

        /// <summary>
        ///     Equivalent words will be treated as blank for the comparison.
        /// </summary>
        private class SpecialBusinessSearchCase
        {
            private readonly string[] equivalentWords;
            private readonly Position position;

            public SpecialBusinessSearchCase(IList<string> equivalentWords, Position position)
            {
                this.equivalentWords = new string[equivalentWords.Count];

                for (var i = 0; i < equivalentWords.Count; i++)
                {
                    this.equivalentWords[i] = equivalentWords[i].ToUpperInvariant().Replace(".", string.Empty);

                    switch (position)
                    {
                        case Position.Start:
                            this.equivalentWords[i] = equivalentWords[i] + " ";
                            break;
                        default:
                            this.equivalentWords[i] = " " + equivalentWords[i];
                            break;
                    }
                }

                this.position = position;
            }

            public void CleanseSpecialCases(ref KeyValuePair<string, Guid> name)
            {
                foreach (var equivalentWord in equivalentWords)
                {
                    switch (position)
                    {
                        case Position.Start:
                            if (name.Key.StartsWith(equivalentWord))
                            {
                                name = new KeyValuePair<string, Guid>(name.Key.Replace(equivalentWord, string.Empty),
                                    name.Value);
                                return;
                            }
                            break;
                        default:
                            if (name.Key.EndsWith(equivalentWord))
                            {
                                name = new KeyValuePair<string, Guid>(name.Key.Replace(equivalentWord, string.Empty),
                                    name.Value);
                                return;
                            }
                            break;
                    }
                }
            }

            public void CleanseSpecialCases(ref string name)
            {
                var kvp = new KeyValuePair<string, Guid>(name, Guid.Empty);

                CleanseSpecialCases(ref kvp);

                name = kvp.Key;
            }
        }

        private enum Position
        {
            Start,
            End
        }
    }
}