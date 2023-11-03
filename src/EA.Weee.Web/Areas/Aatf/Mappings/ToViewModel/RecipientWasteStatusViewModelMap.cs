namespace EA.Weee.Web.Areas.Aatf.Mappings
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Helpers;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Web.ViewModels.Shared;

    public class RecipientWasteStatusViewModelMap : IMap<RecipientWasteStatusFilterBase, RecipientWasteStatusFilterViewModel>
    {
        public RecipientWasteStatusFilterViewModel Map(RecipientWasteStatusFilterBase source)
        {
            Guard.ArgumentNotNull(() => source, source);
            var viewModel = new RecipientWasteStatusFilterViewModel();

            if (source.NoteStatuseList != null && source.NoteStatuseList.Count > 0)
            {
                var sortedListOfNoteStatus = new List<KeyValuePair<int, string>>();
                for (int count = 0; count < source.NoteStatuseList.Count; count++)
                {
                    sortedListOfNoteStatus.Insert(count, new KeyValuePair<int, string>((int)source.NoteStatuseList[count], source.NoteStatuseList[count].ToString()));
                }
                viewModel.NoteStatusList = new SelectList(sortedListOfNoteStatus, "Key", "Value");
            }
            else
            {
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
                else if (source.AllStatuses)
                {
                    sortedListOfNoteStatus.Insert(0, new KeyValuePair<int, string>((int)NoteStatus.Draft, NoteStatus.Draft.ToString()));
                    sortedListOfNoteStatus.Insert(4, new KeyValuePair<int, string>((int)NoteStatus.Returned, NoteStatus.Returned.ToString()));
                }

                viewModel.NoteStatusList = new SelectList(sortedListOfNoteStatus, "Key", "Value");
            }

            viewModel.RecipientList = source.RecipientList != null ? new SelectList(source.RecipientList, "Id", "DisplayName") :
                new SelectList(new List<EntityIdDisplayNameData>(), "Id", "DisplayName");

            viewModel.WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value");
            viewModel.EvidenceNoteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(EvidenceNoteType)), "Key", "Value");
            viewModel.ReceivedId = source.ReceivedId;
            viewModel.NoteStatusValue = source.NoteStatus;
            viewModel.WasteTypeValue = source.WasteType;
            viewModel.SubmittedBy = source.SubmittedBy;
            viewModel.EvidenceNoteTypeValue = source.EvidenceNoteType;

            viewModel.SubmittedByList = source.SubmittedByList != null ?
                new SelectList(source.SubmittedByList, "Id", "DisplayName") :
                new SelectList(new List<EntityIdDisplayNameData>(), "Id", "DisplayName");

            return viewModel;
        }
    }
}