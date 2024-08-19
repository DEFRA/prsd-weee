namespace EA.Weee.Api.Client.Entities.AddressLookup
{
    using System.Text;

    public class Address 
    {
        public string AddressLine { get; set; }

        public string BuildingNumber { get; set; }

        public string Street { get; set; }

        public string Locality { get; set; }

        public string Town { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public int? XCoordinate { get; set; }

        public int? YCoordinate { get; set; }

        public string Uprn { get; set; }

        public string Match { get; set; }

        public string MatchDescription { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Address {\n");
            sb.Append("  AddressLine: ").Append(AddressLine).Append("\n");
            sb.Append("  BuildingNumber: ").Append(BuildingNumber).Append("\n");
            sb.Append("  Street: ").Append(Street).Append("\n");
            sb.Append("  Locality: ").Append(Locality).Append("\n");
            sb.Append("  Town: ").Append(Town).Append("\n");
            sb.Append("  Postcode: ").Append(Postcode).Append("\n");
            sb.Append("  Country: ").Append(Country).Append("\n");
            sb.Append("  XCoordinate: ").Append(XCoordinate).Append("\n");
            sb.Append("  YCoordinate: ").Append(YCoordinate).Append("\n");
            sb.Append("  Uprn: ").Append(Uprn).Append("\n");
            sb.Append("  Match: ").Append(Match).Append("\n");
            sb.Append("  MatchDescription: ").Append(MatchDescription).Append("\n");
            sb.Append("  Language: ").Append(Language).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
