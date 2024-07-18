namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Text.Json.Serialization;

    public class OrganisationData
    {
        public DateTime DateOfCreation { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}