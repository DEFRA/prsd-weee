namespace EA.Weee.Web.Tests.Unit.TestHelpers.Classes
{
    using System.ComponentModel.DataAnnotations;

    public abstract class TestViewModelBase
    {
        public abstract string NoBaseDisplayAttribute { get; set; }

        public abstract string NoBaseDisplayAttributeOverridden { get; set; }

        [Display(Name = "With base display attribute")]
        public abstract string WithBaseDisplayAttribute { get; set; }

        [Display(Name = "This should be overridden")]
        public abstract string WithBaseDisplayAttributeOverridden { get; set; }
    }
}
