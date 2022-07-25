namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public abstract class ListOfSchemeNotesViewModelBase<T> : ListOfNotesViewModelBase<T> where T : ISchemeManageEvidenceViewModel, IManageEvidenceViewModel, new()
    {
        protected ListOfSchemeNotesViewModelBase(IMapper mapper) : base(mapper)
        {
        }

        public T MapSchemeBase(List<EvidenceNoteData> notes,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            SchemePublicInfo scheme)
        {
            Condition.Requires(notes).IsNotNull();

            var model = MapBase(notes, currentDate, manageEvidenceNoteViewModel);
            model.SchemeInfo = scheme;
            model.CanSchemeManageEvidence = scheme.Status != SchemeStatus.Withdrawn;

            return model;
        }
    }
}