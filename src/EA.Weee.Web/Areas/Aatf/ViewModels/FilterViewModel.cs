namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel;

    public class FilterViewModel
    {
        [DisplayName("Search by reference ID")]
        public string SearchRef { get; set; }

        public bool SearchPerformed
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SearchRef);
            }
        }
    }
}