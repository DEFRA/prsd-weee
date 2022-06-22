namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using ViewModels;
    using Web.ViewModels.Shared.Mapping;

    public class EditDraftReturnNotesViewModelMap : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>, IMap<EvidenceNotesViewModelTransfer, EditDraftReturnedNotesViewModel>
    {
        public EditDraftReturnNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public EditDraftReturnedNotesViewModel Map(EvidenceNotesViewModelTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            return MapBase(source.Notes, source.CurrentDate, source.ManageEvidenceNoteViewModel);
        }
    }
}