namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Attributes;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;

    public class EvidenceNoteViewModel
    {
        private readonly ICategoryValueTotalCalculator categoryValueCalculator;

        [Required(ErrorMessage = "Enter a start date")]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [EvidenceNoteStartDate(nameof(EndDate))]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Enter an end date")]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        [EvidenceNoteEndDate(nameof(StartDate))]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Select a receiving PCS")]
        [Display(Name = "Recipient")]
        public Guid? ReceivedId { get; set; }

        public List<SchemeData> SchemeList { get; set; }

        [RequiredSubmitAction(ErrorMessage = "Select a type of waste")]
        [Display(Name = "Type of waste")]
        public WasteType? WasteTypeValue { get; set; }

        public IEnumerable<SelectListItem> WasteTypeList { get; set; }

        [RequiredSubmitAction(ErrorMessage = "Select actual or protocol")]
        [Display(Name = "Actual or protocol")]
        public Protocol? ProtocolValue { get; set; }

        public IEnumerable<SelectListItem> ProtocolList { get; set; }

        [RequiredTonnageAttribute]
        public IList<EvidenceCategoryValue> CategoryValues { get; set; }

        public EvidenceNoteViewModel()
        {
            categoryValueCalculator = new CategoryValueTotalCalculator();
            AddCategoryValues(new EvidenceCategoryValues());
        }

        public EvidenceNoteViewModel(ICategoryValueTotalCalculator categoryValueCalculator)
        {
            this.categoryValueCalculator = categoryValueCalculator;
            AddCategoryValues(new EvidenceCategoryValues());
        }

        public bool Edit
        {
            get { return CategoryValues.Any(c => c.Id != Guid.Empty); }
        }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        private void AddCategoryValues(EvidenceCategoryValues evidenceCategoryValues)
        {
            CategoryValues = new List<EvidenceCategoryValue>();

            foreach (var categoryValue in evidenceCategoryValues)
            {
                CategoryValues.Add(categoryValue);
            }
        }

        public string ReceivedTotal => categoryValueCalculator.Total(CategoryValues.Select(c => c.Received).ToList());

        public string ReusedTotal => categoryValueCalculator.Total(CategoryValues.Select(c => c.Reused).ToList());

        public ActionEnum Action { get; set; }
    }
}