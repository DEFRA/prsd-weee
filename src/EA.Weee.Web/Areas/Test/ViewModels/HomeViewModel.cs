namespace EA.Weee.Web.Areas.Test.ViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;

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