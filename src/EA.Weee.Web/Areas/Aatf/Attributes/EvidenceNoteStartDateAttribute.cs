namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Helpers;
    using Web.ViewModels.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNoteStartDateAttribute : EvidenceDateValidationBase
    {
        public EvidenceNoteStartDateAttribute(string compareDatePropertyName,
            string approvalDateValidationMessage, string aatfStatusValidationMessage) : base(compareDatePropertyName, approvalDateValidationMessage, aatfStatusValidationMessage)
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

            if (!(validationContext.ObjectInstance is EvidenceNoteViewModel evidenceNoteModel))
            {
                return new ValidationResult("Unable to validate the evidence note details");
            }

            var validateStartDateAgainstConfigDate = ValidateDateAgainstEvidenceNoteSiteSelectionDateFrom(thisDate, "start");

            if (validateStartDateAgainstConfigDate != ValidationResult.Success)
            {
                return validateStartDateAgainstConfigDate;
            }

            if (!WindowHelper.IsDateInComplianceYear(thisDate.Year, currentDate))
            {
                return new ValidationResult("The start date must be within an open compliance year");
            }

            var startDateValid = ValidateStartDate(thisDate, otherDate, currentDate);

            if (startDateValid != ValidationResult.Success)
            {
                return startDateValid;
            }

            if (otherDate.HasValue)
            {
                startDateValid = ValidateStartDateIsInTheSameComplianceYearOfEndDate(thisDate, otherDate.Value);
                if (startDateValid != ValidationResult.Success)
                {
                    return startDateValid;
                }
            }

            return ValidateDateAgainstAatfApprovalDate(thisDate, evidenceNoteModel.OrganisationId, evidenceNoteModel.AatfId);
        }
    }
}