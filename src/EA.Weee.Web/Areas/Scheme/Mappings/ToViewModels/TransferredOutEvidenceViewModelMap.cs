namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared.Mapping;

    public class TransferredOutEvidenceViewModelMap : ListOfNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>, IMap<TransferredOutEvidenceNotesViewModelMap, TransferredOutEvidenceNotesSchemeViewModel>
    {
        public TransferredOutEvidenceViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public TransferredOutEvidenceNotesSchemeViewModel Map(TransferredOutEvidenceNotesViewModelMap source)
        {
            Condition.Requires(source).IsNotNull();

            var model = Map(source.Notes, source.CurrentDate, source.ManageEvidenceNoteViewModel);
            model.OrganisationId = source.OrganisationId;
            model.SchemeName = source.SchemeName;

            return model;
        }
    }
}