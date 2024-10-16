﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using System;
    using CuttingEdge.Conditions;
    using Prsd.Core;

    public class SchemeTabViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public SchemePublicInfo Scheme { get; protected set; }

        public EvidenceNoteSearchDataResult NoteData { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public int SelectedComplianceYear { get; protected set; }

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public SchemeTabViewModelMapTransfer(Guid organisationId,
            EvidenceNoteSearchDataResult noteData,
            SchemePublicInfo scheme,
            DateTime currentDate,
            int selectedComplianceYear,
            int pageNumber,
            int pageSize)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => noteData, noteData);
            Condition.Requires(pageNumber).IsNotLessOrEqual(0);
            Condition.Requires(pageSize).IsNotLessOrEqual(0);

            SelectedComplianceYear = selectedComplianceYear;
            OrganisationId = organisationId;
            NoteData = noteData;
            Scheme = scheme;
            CurrentDate = currentDate;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}