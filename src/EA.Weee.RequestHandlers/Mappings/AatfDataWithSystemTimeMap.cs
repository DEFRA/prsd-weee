﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.AatfReturn;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using AatfStatus = Core.AatfReturn.AatfStatus;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class AatfDataWithSystemTimeMap : IMap<AatfWithSystemDateMapperObject, AatfData>
    {
        private readonly IMap<AatfSimpleMapObject, AatfData> aatfMap;

        public AatfDataWithSystemTimeMap(IMap<AatfSimpleMapObject, AatfData> aatfMap)
        {
            this.aatfMap = aatfMap;
        }

        public AatfData Map(AatfWithSystemDateMapperObject source)
        {
            Condition.Requires(source).IsNotNull();
            
            var aatf = aatfMap.Map(new AatfSimpleMapObject(source.Aatf));

            SetAatfDisplayProperty(source, aatf);

            SetCanCreateEditEvidence(source, aatf);

            return aatf;
        }

        private void SetCanCreateEditEvidence(AatfWithSystemDateMapperObject source, AatfData aatf)
        {
            //AATF is allowed to create / edit
            //1. if approval date and compliance year is valid for the current system year and is approved
            //2. if the aatf compliance year is in the previous year and we are in the january period of the next compliance year and aatf is approved 
            var approvalDateValid = ApprovalDateValid(aatf.ApprovalDate, source.SystemDateTime);
            var canCreateEdit = false;

            if (aatf.FacilityType == FacilityType.Aatf)
            {
                if (WindowHelper.IsDateInComplianceYear(aatf.ComplianceYear, source.SystemDateTime) &&
                    approvalDateValid &&
                    aatf.AatfStatus == AatfStatus.Approved)
                {
                    canCreateEdit = true;
                }
            }

            aatf.CanCreateEditEvidence = canCreateEdit;
        }

        private bool ApprovalDateValid(DateTime? aatfApprovalDate, DateTime systemDate)
        {
            var approvalDateValid = false;

            if (aatfApprovalDate.HasValue)
            {
                var approvalDate = aatfApprovalDate.Value.Date;
                var complianceYearEndDate = new DateTime(systemDate.Year + 1, 1, 31);

                if (approvalDate <= complianceYearEndDate.Date && approvalDate <= systemDate.Date)
                {
                    approvalDateValid = true;
                }
            }

            return approvalDateValid;
        }

        private void SetAatfDisplayProperty(AatfWithSystemDateMapperObject source, AatfData aatf)
        {
            var evidenceSiteDisplay = false;

            if (aatf.FacilityType == FacilityType.Aatf)
            {
                var approvalDateValid = ApprovalDateValid(aatf.ApprovalDate, source.SystemDateTime);

                switch (aatf.HasEvidenceNotes)
                {
                    case true when approvalDateValid:
                    case false when approvalDateValid:
                        evidenceSiteDisplay = true;
                        break;
                }
            }

            aatf.EvidenceSiteDisplay = evidenceSiteDisplay;
        }
    }
}
