namespace EA.Weee.Api.Client.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Country
    {
        public string ISO { get; set; }

        [Display(Name = "Country")]
        public string Name { get; set; }
    }
}
