namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class TransferSelectedDataModel
    {
        public IList<int> SelectedCategoryIds { get; set; }

        public Guid SelectedSchemeId { get; set; }

        public TransferSelectedDataModel(List<int> selectedCategory, Guid schemeId)
        {
            SelectedCategoryIds = selectedCategory;
            SelectedSchemeId = schemeId;
        }
    }
}
