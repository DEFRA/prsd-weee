namespace EA.Weee.Web.Areas.Test.ViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    
    public class HomeViewModel
    {
        //TODO: These strings should be loaded from a resource file.
        public const string OptionGeneratePcsXmlFile = "Generate PCS XML File";
        
        [Required]
        public RadioButtonStringCollectionViewModel Options { get; set; }

        public HomeViewModel()
        {
            List<string> collection = new List<string>
            {
                OptionGeneratePcsXmlFile,
            };

            Options = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}