namespace EA.Weee.Web.Areas.Aatf.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Helpers;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    public class RecipientWasteStatusViewModelMap : IMap<RecipientWasteStatusFilterBase, RecipientWasteStatusFilterViewModel>
    {
        public RecipientWasteStatusFilterViewModel Map(RecipientWasteStatusFilterBase source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var sortedmNoteStatusList = new Dictionary<int, string>
            {
                { (int)NoteStatus.Approved, NoteStatus.Approved.ToString() },
                { (int)NoteStatus.Submitted, NoteStatus.Submitted.ToString() },
                { (int)NoteStatus.Rejected, NoteStatus.Rejected.ToString() },
                { (int)NoteStatus.Void, NoteStatus.Void.ToString() },
            };

            return new RecipientWasteStatusFilterViewModel()
            {   
                SchemeListSL = new SelectList(source.SchemeList, "Id", "SchemeName"),
                NoteStatusList = new SelectList(sortedmNoteStatusList, "Key", "Value"),
                WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value"),
                ReceivedId = source.ReceivedId,
                NoteStatusValue = source.NoteStatus,
                WasteTypeValue = source.WasteType
            };
        }
    }
}
