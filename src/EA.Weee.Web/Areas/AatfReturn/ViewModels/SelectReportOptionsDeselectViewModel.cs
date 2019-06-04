namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;

    public class SelectReportOptionsDeselectViewModel : SelectReportOptionsModelBase
    {
        public SelectReportOptionsDeselectViewModel()
        {
        }

        [Required(ErrorMessage = "You must tell us if you want to remove these reporting options")]
        public string SelectedValue { get; set; }

        public IList<string> PossibleValues => new List<string> { "Yes", "No" };
    }
}