namespace EA.Weee.Web.Areas.Aatf.Mappings
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Helpers;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    public class RecipientWasteStatusViewModelMap : IMap<RecipientWasteStatusFilterBase, RecipientWasteStatusFilterViewModel>
    {
        public RecipientWasteStatusFilterViewModel Map(RecipientWasteStatusFilterBase source)
        {
            Guard.ArgumentNotNull(() => source, source);
            var viewModel = new RecipientWasteStatusFilterViewModel();

            var sortedListOfNoteStatus = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>((int)NoteStatus.Submitted, NoteStatus.Submitted.ToString()),
                new KeyValuePair<int, string>((int)NoteStatus.Approved, NoteStatus.Approved.ToString()),
                new KeyValuePair<int, string>((int)NoteStatus.Rejected, NoteStatus.Rejected.ToString()),
                new KeyValuePair<int, string>((int)NoteStatus.Void, NoteStatus.Void.ToString()),
            };

            if (source.Internal)
            {
                sortedListOfNoteStatus.Insert(3, new KeyValuePair<int, string>((int)NoteStatus.Returned, NoteStatus.Returned.ToString()));
            }

            if (source.AllStatuses)
            {
                sortedListOfNoteStatus.Insert(0, new KeyValuePair<int, string>((int)NoteStatus.Draft, NoteStatus.Draft.ToString()));
                sortedListOfNoteStatus.Insert(4, new KeyValuePair<int, string>((int)NoteStatus.Returned, NoteStatus.Returned.ToString()));
            }

            viewModel.RecipientList = source.RecipientList != null ? new SelectList(source.RecipientList, "Id", "DisplayName") : 
                new SelectList(new List<EntityIdDisplayNameData>(), "Id", "DisplayName");

            viewModel.NoteStatusList = new SelectList(sortedListOfNoteStatus, "Key", "Value");
            viewModel.WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value");
            viewModel.ReceivedId = source.ReceivedId;
            viewModel.NoteStatusValue = source.NoteStatus;
            viewModel.WasteTypeValue = source.WasteType;
            viewModel.SubmittedBy = source.SubmittedBy;

            viewModel.SubmittedByList = source.SubmittedByList != null ? new SelectList(source.SubmittedByList, "Id", "DisplayName") :
         new SelectList(new List<EntityIdDisplayNameData>(), "Id", "DisplayName");

            return viewModel;
        }
    }
}
