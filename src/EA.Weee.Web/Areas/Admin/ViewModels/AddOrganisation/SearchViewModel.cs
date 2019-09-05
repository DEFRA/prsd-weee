namespace EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using Core.Shared;
    using EA.Weee.Core.Helpers;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        [MaxLength(CommonMaxFieldLengths.DefaultString)]
        public string SearchTerm { get; set; }

        public EntityType EntityType { get; set; }

        public bool IsAeOrAatf => EntityType == EntityType.Aatf || EntityType == EntityType.Ae;

        public string EntityTypeFormatted()
        {
            if (this.EntityType == EntityType.Pcs)
            {
                return "a " + EntityType.Pcs.ToDisplayString();
            }

            return "an " + EntityType.ToDisplayString();
        }
    }
}