namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using System.Collections.Generic;
    using Core.Aatf;

    public class SchemeObligationViewModel
    {
        public string UpdateDate { get; set; }

        public string SchemeName { get; set; }

        public List<CategoryValues<SchemeObligationValue>> ObligationCategoryValue { get; set; }
    }
}