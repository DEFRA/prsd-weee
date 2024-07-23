namespace EA.Weee.Web.Controllers
{
    public class DefraCompaniesHouseApiModel
    {
        public string RegistrationNumber { get; set; }

        public Organisation Organisation { get; set; }

        public Meta _meta { get; set; }

        public Info _info { get; set; }
    }
}