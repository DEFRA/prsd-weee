﻿namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    [Serializable]
    public class ManageEvidenceNoteTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public Guid AatfId { get; protected set; }

        public AatfData AatfData { get; protected set; }

        public List<AatfData> Aatfs { get; protected set; }

        public FilterViewModel FilterViewModel { get; protected set; }

        public RecipientWasteStatusFilterViewModel RecipientWasteStatusFilterViewModel { get; protected set; }

        public SubmittedDatesFilterViewModel SubmittedDatesFilterViewModel { get; protected set; }

        public IEnumerable<int> ComplianceYearList { get; protected set; }

        public int ComplianceYear { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteTransfer(Guid organisationId, Guid aatfId, AatfData aatfData, List<AatfData> aatfs,
            FilterViewModel filterViewModel, RecipientWasteStatusFilterViewModel recipientWasteStatusFilterViewModel,
            SubmittedDatesFilterViewModel submittedDatesFilterViewModel, int complianceYear, DateTime currentDate)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => aatfData, aatfData);
            Guard.ArgumentNotNull(() => aatfs, aatfs);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            OrganisationId = organisationId;
            AatfData = aatfData;
            Aatfs = aatfs;
            AatfId = aatfId;
            FilterViewModel = filterViewModel;
            RecipientWasteStatusFilterViewModel = recipientWasteStatusFilterViewModel;
            SubmittedDatesFilterViewModel = submittedDatesFilterViewModel;
            ComplianceYear = complianceYear;
            CurrentDate = currentDate;
        }

        public ManageEvidenceNoteTransfer(Guid organisationId,
            FilterViewModel filterViewModel, RecipientWasteStatusFilterViewModel recipientWasteStatusFilterViewModel,
            SubmittedDatesFilterViewModel submittedDatesFilterViewModel, int complianceYear, DateTime currentDate)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            
            OrganisationId = organisationId;
            FilterViewModel = filterViewModel;
            RecipientWasteStatusFilterViewModel = recipientWasteStatusFilterViewModel;
            SubmittedDatesFilterViewModel = submittedDatesFilterViewModel;
            ComplianceYear = complianceYear;
            CurrentDate = currentDate;
        }
    }
}