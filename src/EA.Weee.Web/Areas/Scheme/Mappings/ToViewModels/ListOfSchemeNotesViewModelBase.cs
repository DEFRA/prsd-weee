namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using Services;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public abstract class ListOfSchemeNotesViewModelBase<T> : ListOfNotesViewModelBase<T> where T : ISchemeManageEvidenceViewModel, 
        IManageEvidenceViewModel, new()
    {
        protected ListOfSchemeNotesViewModelBase(IMapper mapper) : base(mapper)
        {
        }

        public T MapSchemeBase(EvidenceNoteSearchDataResult noteData,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            SchemePublicInfo scheme,
            int pageNumber,
            int pageSize)
        {
            Condition.Requires(noteData).IsNotNull();

            var model = MapBase(noteData, currentDate, manageEvidenceNoteViewModel, pageNumber, pageSize);
            model.SchemeInfo = scheme;
            model.CanSchemeManageEvidence = scheme != null && 
                                            scheme.Status != SchemeStatus.Withdrawn && 
                                            !model.ManageEvidenceNoteViewModel.ComplianceYearClosed;
            return model;
        }
    }
}