namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using Core.Helpers;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public abstract class ListOfSchemeNotesViewModelBase<T> : ListOfNotesViewModelBase<T> where T : ISchemeManageEvidenceViewModel, 
        IManageEvidenceViewModel, new()
    {
        protected ListOfSchemeNotesViewModelBase(IMapper mapper) : base(mapper)
        {
        }

        public T MapSchemeBase(EvidenceNoteSearchDataResult noteData,
            SchemePublicInfo scheme,
            DateTime currentDate,
            int complianceYear,
            int pageNumber,
            int pageSize)
        {
            Condition.Requires(noteData).IsNotNull();

            var model = MapBase(noteData, pageNumber, pageSize);

            model.CanSchemeManageEvidence = scheme != null &&
                                            scheme.Status != SchemeStatus.Withdrawn &&
                                            WindowHelper.IsDateInComplianceYear(complianceYear, currentDate);
            model.SchemeInfo = scheme;

            return model;
        }
    }
}