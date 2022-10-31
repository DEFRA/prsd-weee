namespace EA.Weee.Web.ViewModels.Shared
{
    using System.ComponentModel;

    public class FilterViewModel
    {
        [DisplayName("Search by reference ID")]
        public string SearchRef { get; set; }

        public bool SearchPerformed => !string.IsNullOrWhiteSpace(SearchRef);
    }
}