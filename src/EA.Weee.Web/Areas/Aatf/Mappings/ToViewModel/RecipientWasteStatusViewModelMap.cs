namespace EA.Weee.Web.Areas.Aatf.Mappings
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Helpers;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    public class RecipientWasteStatusViewModelMap : IMap<RecipientWasteStatusFilterBase, RecipientWasteStatusFilterViewModel>
    {
        public RecipientWasteStatusFilterViewModel Map(RecipientWasteStatusFilterBase source)
        {
            Guard.ArgumentNotNull(() => source, source);
            var viewModel = new RecipientWasteStatusFilterViewModel();

            var sortedmNoteStatusList = new Dictionary<int, string>
            {
                { (int)NoteStatus.Submitted, NoteStatus.Submitted.ToString() },
                { (int)NoteStatus.Approved, NoteStatus.Approved.ToString() },
                { (int)NoteStatus.Rejected, NoteStatus.Rejected.ToString() },
                { (int)NoteStatus.Void, NoteStatus.Void.ToString() },
            };

            if (source.SchemeList != null)
            {
                viewModel.SchemeListSL = new SelectList(source.SchemeList, "Id", "SchemeName");
            }
            else
            {
               viewModel.SchemeListSL = new SelectList(new List<SchemeData>(), "Id", "SchemeName");
            }

            viewModel.NoteStatusList = new SelectList(sortedmNoteStatusList, "Key", "Value");
            viewModel.WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value");
            viewModel.ReceivedId = source.ReceivedId;
            viewModel.NoteStatusValue = source.NoteStatus;
            viewModel.WasteTypeValue = source.WasteType;
       
            return viewModel;
        }
    }
}
