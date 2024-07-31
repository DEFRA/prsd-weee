namespace EA.Weee.Web.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class Country
    {
        public string ISO { get; set; }

        [Display(Name = "Country")]
        public string Name { get; set; }
    }
}