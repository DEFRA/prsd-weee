namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Domain.Evidence;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class TransferEvidenceDataViewModel
    {
        public int SelectedNoteId { get; set; }
        public Note SelectedNote { get; set; }
        public IEnumerable<SelectListItem> NotesToDisplay { get; set; }
    }
}