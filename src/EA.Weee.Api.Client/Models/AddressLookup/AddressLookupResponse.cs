namespace EA.Weee.Api.Client.Models.AddressLookup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class AddressLookupResponse
    {
        [JsonPropertyName("header")]
        public Header Header { get; set; }

        [JsonPropertyName("results")]
        public List<AddressResult> Results { get; set; }

        [JsonPropertyName("_info")]
        public RequestInfo Info { get; set; }

        [JsonIgnore]
        public bool InvalidRequest { get; set; }

        [JsonIgnore]
        public bool Error { get; set; }

        public bool HasValidResults => !InvalidRequest && !Error && Results?.Any() == true;

        [JsonIgnore]
        public bool SearchTooBroad { get; set; }
    }
}
