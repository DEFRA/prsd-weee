namespace EA.Weee.Web.Areas.Test.ViewModels
{
    using System.Collections.Generic;
    using EA.Weee.Web.ViewModels.Shared;

    public class HomeViewModel : RadioButtonStringCollectionViewModel
    {
        public const string OptionGeneratePcsXmlFile = "Generate PCS XML File";
        public const string ManageCache = "Manage Cache";

        public HomeViewModel() : base(new List<string>
            {
                OptionGeneratePcsXmlFile,
                ManageCache
            })
        {
        }
    }
}