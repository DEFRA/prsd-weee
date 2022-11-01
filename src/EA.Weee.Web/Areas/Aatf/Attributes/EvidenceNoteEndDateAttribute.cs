namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Helpers;
    using Web.ViewModels.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteEndDateAttribute : EvidenceDateValidationBase
    {
        public EvidenceNoteEndDateAttribute(string compareDatePropertyName, 
            string approvalDateValidationMessage, string aatfStatusValidationMessage) : 
            base(compareDatePropertyName, approvalDateValidationMessage, aatfStatusValidationMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentDate = GetCurrentDateTime();

            if (value == null)
            {
                // let the required field validation deal with the entering of the date
                return ValidationResult.Success;
            }

            var thisDate = ((DateTime)value).Date;
            var otherDate = (DateTime?)validationContext.ObjectType.GetProperty(CompareDatePropertyName)?.GetValue(validationContext.ObjectInstance, null);

            var validateEndDateAgainstConfigDate = ValidateDateAgainstEvidenceNoteSiteSelectionDateFrom(thisDate, "end");

            if (validateEndDateAgainstConfigDate != ValidationResult.Success)
            {
                return validateEndDateAgainstConfigDate;
            }

            if (!WindowHelper.IsDateInComplianceYear(thisDate.Year, currentDate))
            {
                return new ValidationResult("The end date must be within an open compliance year");
            }

            if (!(validationContext.ObjectInstance is EvidenceNoteViewModel evidenceNoteModel))
            {
                return new ValidationResult("Unable to validate the evidence note details");
            }

            var endDateValid = ValidateEndDate(otherDate, thisDate);

            if (endDateValid != ValidationResult.Success)
            {
                return endDateValid;
            }

            return ValidateDateAgainstAatfApprovalDate(thisDate, evidenceNoteModel.OrganisationId, evidenceNoteModel.AatfId);
        }
    }
}