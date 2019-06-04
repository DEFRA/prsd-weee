namespace EA.Weee.Web.Areas.AeReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ExportedWholeWeeeViewModel
    {
        public IList<string> PossibleValues => new List<string> { "Yes", "No" };

        public string WeeeSelectedValue { get; set; }
    }
}