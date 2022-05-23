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

            //TODO: place holder map values until the compliance year is put in for AATF
            return Map(source.Notes, new DateTime(), null);
        }
    }
}