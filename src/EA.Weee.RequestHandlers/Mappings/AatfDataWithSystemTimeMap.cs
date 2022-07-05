namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;
    using Domain.AatfReturn;
    using Prsd.Core.Mapper;
    using FacilityType = Domain.AatfReturn.FacilityType;

    public class AatfDataWithSystemTimeMap : IMap<AatfWithSystemDateMapperObject, AatfData>
    {
        private readonly IMap<Aatf, AatfData> aatfMap;

        public AatfDataWithSystemTimeMap(IMap<Aatf, AatfData> aatfMap)
        {
            this.aatfMap = aatfMap;
        }

        public AatfData Map(AatfWithSystemDateMapperObject source)
        {
            Condition.Requires(source).IsNotNull();
            
            var aatf = aatfMap.Map(source.Aatf);

            var evidenceSiteDisplay = false;
            var approvalDateValid = false;

            if (aatf.FacilityType == Core.AatfReturn.FacilityType.Aatf)
            {
                if (aatf.ApprovalDate.HasValue)
                {
                    var complianceYearEndDate = new DateTime(source.SystemDateTime.Year + 1, 1, 31);
                    var approvalDate = aatf.ApprovalDate.Value.Date;

                    if (approvalDate <= complianceYearEndDate.Date && approvalDate <= source.SystemDateTime.Date)
                    {
                        approvalDateValid = true;
                    }
                }

                switch (aatf.HasEvidenceNotes)
                {
                    case true when approvalDateValid:
                    case false when approvalDateValid:
                        evidenceSiteDisplay = true;
                        break;
                }
            }

            aatf.EvidenceSiteDisplay = evidenceSiteDisplay;

            return aatf;
        }
    }
}
