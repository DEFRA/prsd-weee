namespace EA.Weee.Web.Controllers
{
    public class CompaniesHouseModel
    {
        public DefraCompaniesHouseApiModel DefraCompaniesHouseApiModel { get; set; }

        public CompaniesHouseApiModel CompaniesHouseApiModel { get; set; }

        public string RegistrationNumber { get; set; }

        public string Error { get; set; }

        public bool UseDefra { get; set; }

        public string Response { get; set; }
    }
}