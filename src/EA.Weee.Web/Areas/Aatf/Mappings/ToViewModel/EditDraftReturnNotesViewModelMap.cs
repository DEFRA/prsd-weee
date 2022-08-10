namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Web.ViewModels.Shared.Mapping;

    public class EditDraftReturnNotesViewModelMap : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>, IMap<EvidenceNotesViewModelTransfer, EditDraftReturnedNotesViewModel>
    {
        public EditDraftReturnNotesViewModelMap(IMapper mapper, ConfigurationService configurationService) : base(mapper, configurationService)
        {
        }

        public EditDraftReturnedNotesViewModel Map(EvidenceNotesViewModelTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            return MapBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.PageNumber);
        }
    }
}