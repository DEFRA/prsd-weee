namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class EditDraftReturnNotesViewModelMap : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>, IMap<EvidenceNotesViewModelTransfer, EditDraftReturnedNotesViewModel>
    {
        public EditDraftReturnNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public EditDraftReturnedNotesViewModel Map(EvidenceNotesViewModelTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            return Map(source.Notes, SystemTime.UtcNow, null);
        }
    }
}