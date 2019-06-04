namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ManageAatfsViewModel : ManageFacilityModelBase, IValidatableObject
    {
        public List<AatfDataList> AatfDataList { get; set; }

        public FilteringViewModel Filter { get; set; }

        public override Guid? Selected { get; set; }

        public bool CanAddAatf { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Selected == null)
            {
                yield return new ValidationResult(string.Format("You must select an {0} to manage", this.FacilityType.ToString().ToUpper()), new[] { "Selected" });
            }
        }
    }
}