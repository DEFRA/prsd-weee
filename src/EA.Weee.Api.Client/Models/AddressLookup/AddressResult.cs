namespace EA.Weee.Api.Client.Models.AddressLookup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json.Serialization;

    public class AddressResult
    {
        [JsonPropertyName("addressLine")] public string AddressLine { get; set; }

        [JsonPropertyName("buildingNumber")] public string BuildingNumber { get; set; }

        [JsonPropertyName("subBuildingName")] public string SubBuildingName { get; set; }

        [JsonPropertyName("buildingName")] public string BuildingName { get; set; }

        [JsonPropertyName("street")] public string Street { get; set; }

        [JsonPropertyName("town")] public string Town { get; set; }

        [JsonPropertyName("administrativeArea")]
        public string AdministrativeArea { get; set; }

        [JsonPropertyName("historicCounty")] public string HistoricCounty { get; set; }

        [JsonPropertyName("ceremonialCounty")] public string CeremonialCounty { get; set; }

        [JsonPropertyName("postcode")] public string Postcode { get; set; }

        [JsonPropertyName("country")] public string Country { get; set; }

        [JsonPropertyName("xCoordinate")] public int XCoordinate { get; set; }

        [JsonPropertyName("yCoordinate")] public int YCoordinate { get; set; }

        [JsonPropertyName("uprn")] public string Uprn { get; set; }

        [JsonPropertyName("match")] public string Match { get; set; }

        [JsonPropertyName("matchDescription")] public string MatchDescription { get; set; }

        [JsonPropertyName("language")] public string Language { get; set; }

        public string FormattedAddress
        {
            get
            {
                var addressParts = new List<string>();

                // Add building number and street
                var buildingAndStreet = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(BuildingNumber))
                {
                    buildingAndStreet.Append(BuildingNumber);
                }

                if (!string.IsNullOrWhiteSpace(Street))
                {
                    if (buildingAndStreet.Length > 0)
                    {
                        buildingAndStreet.Append(" ");
                    }

                    buildingAndStreet.Append(Street);
                }

                if (buildingAndStreet.Length > 0)
                {
                    addressParts.Add(buildingAndStreet.ToString());
                }

                // Add town if different from administrative area
                if (!string.IsNullOrWhiteSpace(Town) &&
                    !string.Equals(Town, AdministrativeArea, StringComparison.OrdinalIgnoreCase))
                {
                    addressParts.Add(Town);
                }

                // Add administrative area if present
                if (!string.IsNullOrWhiteSpace(AdministrativeArea))
                {
                    addressParts.Add(AdministrativeArea);
                }

                // Add county (prefer ceremonial over historic if both present)
                var county = !string.IsNullOrWhiteSpace(CeremonialCounty)
                    ? CeremonialCounty
                    : HistoricCounty;
                if (!string.IsNullOrWhiteSpace(county))
                {
                    addressParts.Add(county);
                }

                // Add postcode
                if (!string.IsNullOrWhiteSpace(Postcode))
                {
                    addressParts.Add(Postcode);
                }

                // Add country if not England (as it's typically omitted for English addresses)
                if (!string.IsNullOrWhiteSpace(Country) &&
                    !string.Equals(Country, "ENGLAND", StringComparison.OrdinalIgnoreCase))
                {
                    addressParts.Add(Country);
                }

                return string.Join(", ", addressParts.Where(p => !string.IsNullOrWhiteSpace(p)));
            }
        }
    }
}
