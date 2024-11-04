namespace EA.Weee.Web.Areas.Test.ViewModels.ApiIntegration
{
    using System.ComponentModel.DataAnnotations;

    public class ApiModel
    {
        public string Result { get; set; }

        public string PostcodeValue { get; set; }

        public string CompanyValue { get; set; }

        // Property to track which form was submitted
        public string SubmittedForm { get; set; }
    }
}