﻿namespace EA.Weee.Web.Areas.AatfEvidence.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Scheme;

    public class CreateNoteViewModel
    {
        [Required]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Recipient")]
        public Guid? ReceivedId { get; set; }

        public List<SchemeData> SchemeList { get; set; }

        [Display(Name = "Type of waste")]
        public int WasteTypeValue { get; set; }

        public IEnumerable<WasteType> WasteTypeList { get; set; }

        [Display(Name = "Actual or protocol")]
        public int ProtocolValue { get; set; }

        public IEnumerable<Protocol> ProtocolList { get; set; }

        public IList<EvidenceCategoryValue> CategoryValues { get; set; }

        public CreateNoteViewModel()
        {
            AddCategoryValues(new EvidenceCategoryValues());
        }

        public bool Edit
        {
            get { return CategoryValues.Any(c => c.Id != Guid.Empty); }
        }

        private void AddCategoryValues(EvidenceCategoryValues evidenceCategoryValues)
        {
            CategoryValues = new List<EvidenceCategoryValue>();

            foreach (var categoryValue in evidenceCategoryValues)
            {
                CategoryValues.Add(categoryValue);
            }
        }
    }
}