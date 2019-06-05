namespace EA.Weee.Web.Areas.AeReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class ExportedWholeWeeeViewModel : IValidatableObject
    {
        public YesNoEnum WeeeSelectedValue { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.WeeeSelectedValue == YesNoEnum.NotSelected)
            {
                yield return new ValidationResult("You must select an option", new[] { "WeeeSelectedValue" });
            }
        }
    }

    public enum YesNoEnum
    {
        NotSelected,
        Yes,
        No
    }
}