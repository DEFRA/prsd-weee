namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mapper;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels;
    using Web.ViewModels.Shared.Mapping;

    public class EditNoteViewModelMap : IMap<EditNoteMapTransfer, EditEvidenceNoteViewModel>
    {
        public EditEvidenceNoteViewModel Map(EditNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var recipientId = source.ExistingModel != null ? source.ExistingModel.RecipientId : source.NoteData.RecipientOrganisationData.Id;
            var rejectedReason = source.ExistingModel != null
                ? source.ExistingModel.RejectedReason
                : source.NoteData.RejectedReason;
            var returnedReason = source.ExistingModel != null
                ? source.ExistingModel.ReturnedReason
                : source.NoteData.ReturnedReason;

            var recipientName = source.Schemes.FirstOrDefault(s => s.OrganisationId == recipientId);

            var model = new EditEvidenceNoteViewModel
            {
                Id = source.ExistingModel?.Id ?? source.NoteData.Id,
                Status = source.ExistingModel?.Status ?? source.NoteData.Status,
                Reference = source.ExistingModel?.Reference ?? source.NoteData.Reference,
                Type = source.ExistingModel?.Type ?? source.NoteData.Type,
                OrganisationId = source.ExistingModel?.OrganisationId ?? source.OrganisationId,
                AatfId = source.ExistingModel?.AatfId ?? source.AatfId,
                SchemeList = source.Schemes,
                ProtocolList = new SelectList(EnumHelper.GetValues(typeof(Protocol)), "Key", "Value"),
                WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value"),
                RejectedReason = rejectedReason,
                ReturnedReason = returnedReason,
                SelectedSchemeName = recipientName == null ? string.Empty : recipientName.DisplayName
            };

            if (source.ExistingModel != null)
            {
                model.RecipientId = source.ExistingModel.RecipientId;
                model.StartDate = source.ExistingModel.StartDate;
                model.EndDate = source.ExistingModel.EndDate;
                model.WasteTypeValue = source.ExistingModel.WasteTypeValue;
                model.ProtocolValue = source.ExistingModel.ProtocolValue;
                model.CategoryValues = source.ExistingModel.CategoryValues;
            }
            else
            {
                model.RecipientId = source.NoteData.RecipientSchemeData.Id;
                model.StartDate = source.NoteData.StartDate;
                model.EndDate = source.NoteData.EndDate;
                model.WasteTypeValue = source.NoteData.WasteType;
                model.ProtocolValue = source.NoteData.Protocol;

                foreach (var evidenceTonnageData in source.NoteData.EvidenceTonnageData)
                {
                    var category = model.CategoryValues.FirstOrDefault(c => c.CategoryId.Equals(evidenceTonnageData.CategoryId.ToInt()));

                    if (category != null)
                    {
                        category.Received = evidenceTonnageData.Received.ToTonnageEditDisplay();
                        category.Reused = evidenceTonnageData.Reused.ToTonnageEditDisplay();
                        category.Id = evidenceTonnageData.Id;
                    }
                }
            }

            return model;
        }
    }
}