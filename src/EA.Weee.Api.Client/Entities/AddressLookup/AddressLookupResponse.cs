namespace EA.Weee.Api.Client.Entities.AddressLookup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Remoting.Messaging;
    using System.Text;

    public class AddressLookupResponse
    {
        [Required]
        public Header Header { get; set; }

        [Required]
        public List<Address> Results { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Response200Results {\n");
            sb.Append("  Header: ").Append(Header).Append("\n");
            sb.Append("  Results: ").Append(Results).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
