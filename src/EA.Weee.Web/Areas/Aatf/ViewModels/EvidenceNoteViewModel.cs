namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Attributes;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;

    public class EvidenceNoteViewModel
    {
        private readonly ICategoryValueTotalCalculator categoryValueCalculator;

        [Required]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [EvidenceNoteStartDate(nameof(EndDate))]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        [EvidenceNoteEndDate(nameof(StartDate))]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Recipient")]
        public Guid? ReceivedId { get; set; }

        public List<SchemeData> SchemeList { get; set; }

        [Display(Name = "Type of waste")]
        public int? WasteTypeValue { get; set; }

        public IEnumerable<WasteType> WasteTypeList { get; set; }

        [Display(Name = "Actual or protocol")]
        public int? ProtocolValue { get; set; }

        public IEnumerable<Protocol> ProtocolList { get; set; }

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
    }
}