namespace EA.Weee.Web.Controllers
{
    using System.Text.Json.Serialization;
    public class RootObject
    {
        public Organisation Organisation { get; set; }
        public Meta _meta { get; set; }
        public Info _info { get; set; }
    }
}