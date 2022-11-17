namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Attributes;
    using Core.AatfEvidence;
    using Core.Helpers;
    using EA.Weee.Core.Shared;
    using Web.ViewModels.Shared;

    [Serializable]
    public class EditEvidenceNoteViewModel : EvidenceNoteViewModel, IActionModel
    {
        [Required(ErrorMessage = "Enter a start date")]
        [Display(Name = "Start date")]
        [EvidenceNoteStartDate(nameof(EndDate), "Enter a start date on or after the date of your AATF approval", "You cannot create evidence because your site approval has been cancelled or suspended, or your site is not approved for the start date entered")]
        [DataType(DataType.Date)]
        public override DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Enter an end date")]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        [EvidenceNoteEndDate(nameof(StartDate), "Enter an end date on or after the date of your AATF approval", "You cannot create evidence because your site approval has been cancelled or suspended, or your site is not approved for the end date entered")]
        public override DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Select a recipient")]
        [Display(Name = "Recipient")]
        public Guid? RecipientId { get; set; }

        public List<EntityIdDisplayNameData> SchemeList { get; set; }

        [Display(Name = "Recipient")]
        public string SelectedSchemeName { get; set; }

        [RequiredSubmitAction(ErrorMessage = "Select an obligation type")]
        [EvidenceNotePbsHouseHoldValidation(nameof(RecipientId), ErrorMessage = "You cannot issue non-household evidence to the PBS. Select household.")]
        [Display(Name = "Obligation type")]

        public override WasteType? WasteTypeValue { get; set; }

        public IEnumerable<SelectListItem> WasteTypeList { get; set; }

        [RequiredSubmitAction(ErrorMessage = "Select actual or protocol")]
        [Display(Name = "Actual or protocol")]
        public override Protocol? ProtocolValue { get; set; }

        public IEnumerable<SelectListItem> ProtocolList { get; set; }

        [RequiredTonnage]
        public override IList<EvidenceCategoryValue> CategoryValues { get; set; }

        public bool Edit => Id != Guid.Empty;

        public ActionEnum Action { get; set; }

        public bool ReturnToView { get; set; }

        public EditEvidenceNoteViewModel() : base()
        {
        }

        public EditEvidenceNoteViewModel(ICategoryValueTotalCalculator categoryValueCalculator) : base(categoryValueCalculator)
        {
        }

        public static IEnumerable<string> ValidationMessageDisplayOrder => new List<string>
        {
            nameof(StartDate),
            $"{nameof(EndDate)}",
            $"{nameof(RecipientId)}",
            "Recipient-auto",
            $"{nameof(WasteTypeValue)}",
            $"{nameof(ProtocolValue)}"
        };
    }
}