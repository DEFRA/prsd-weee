﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared.Mapping;

    public class TransferredOutEvidenceViewModelMap : ListOfSchemeNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>, IMap<TransferredOutEvidenceNotesViewModelMapTransfer, TransferredOutEvidenceNotesSchemeViewModel>
    {
        public TransferredOutEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public TransferredOutEvidenceNotesSchemeViewModel Map(TransferredOutEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = MapSchemeBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.Scheme);
            model.OrganisationId = source.OrganisationId;
           
            foreach (var evidenceNoteRowViewModel in model.EvidenceNotesDataList)
            {
                evidenceNoteRowViewModel.DisplayEditLink = evidenceNoteRowViewModel.Status == NoteStatus.Draft ||
                                                           evidenceNoteRowViewModel.Status == NoteStatus.Returned &&  //#48724
                                                           model.CanSchemeManageEvidence;
            }

            return model;
        }
    }
}