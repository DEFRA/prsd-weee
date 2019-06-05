namespace EA.Weee.Web.Tests.Unit.TestHelpers.Classes
{
    using System.ComponentModel.DataAnnotations;

    public class TestViewModel : TestViewModelBase
    {
        public override string NoBaseDisplayAttribute { get; set; }

        [Display(Name = "No base display attribute overridden")]
        public override string NoBaseDisplayAttributeOverridden { get; set; }

        public override string WithBaseDisplayAttribute { get; set; }

        [Display(Name = "With base display attribute overridden")]
        public override string WithBaseDisplayAttributeOverridden { get; set; }
    }
}
