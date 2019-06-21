namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Scheme;
    using Domain.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Aatf = Core.AatfReturn.AatfData;
    using ReturnReportOn = Core.AatfReturn.ReturnReportOn;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    public class ReturnMap : IMap<ReturnQuarterWindow, ReturnData>
    {
        private readonly IMapper mapper;
        private readonly IMap<Organisation, OrganisationData> organisationMapper;

        public ReturnMap(IMapper mapper, IMap<Organisation, OrganisationData> organisationMapper)
        {
            this.mapper = mapper;
            this.organisationMapper = organisationMapper;
        }

        public ReturnData Map(ReturnQuarterWindow source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var returnData = new ReturnData()
            {
                Id = source.Return.Id,
                Quarter = new Quarter(source.Return.Quarter.Year, (QuarterType)source.Return.Quarter.Q),
                QuarterWindow = new QuarterWindow(source.QuarterWindow.StartDate, source.QuarterWindow.EndDate),
                OrganisationData = organisationMapper.Map(source.Organisation),
                SchemeDataItems = source.ReturnSchemes.Select(s => mapper.Map<EA.Weee.Domain.Scheme.Scheme, SchemeData>(s.Scheme)).ToList(),
                CreatedBy = source.Return.CreatedBy.FullName,
                CreatedDate = source.Return.CreatedDate,
                SubmittedBy = source.Return.SubmittedBy?.FullName,
                SubmittedDate = source.Return.SubmittedDate,
                ReturnReportOns = source.ReturnReportOns.Select(r => new ReturnReportOn(r.ReportOnQuestionId, r.ReturnId)).ToList(),
                ReturnStatus = mapper.Map<ReturnStatus>(source.Return.ReturnStatus),
                NilReturn = source.Return.NilReturn
            };

            if (source.Aatfs != null)
            {
                var aatfReturnList = new List<Aatf>();

                foreach (var sourceAatf in source.Aatfs)
                {
                    aatfReturnList.Add(new Aatf(sourceAatf.Id, sourceAatf.Name, sourceAatf.ApprovalNumber, sourceAatf.ComplianceYear));
                }

                returnData.Aatfs = aatfReturnList;
            }

            if (source.NonObligatedWeeeList != null)
            {
                returnData.NonObligatedData = source.NonObligatedWeeeList.Select(n => new NonObligatedData(n.CategoryId, n.Tonnage, n.Dcf, n.Id)).ToList();
            }

            if (source.ObligatedWeeeReceivedList != null)
            {
                returnData.ObligatedWeeeReceivedData = source.ObligatedWeeeReceivedList.Select(n => new WeeeObligatedData(n.Id,
                    new Scheme(n.WeeeReceived.Scheme.Id, n.WeeeReceived.Scheme.SchemeName),
                    new Aatf(n.WeeeReceived.Aatf.Id, n.WeeeReceived.Aatf.Name, n.WeeeReceived.Aatf.ApprovalNumber, n.WeeeReceived.Aatf.ComplianceYear),
                    n.CategoryId,
                    n.NonHouseholdTonnage,
                    n.HouseholdTonnage)).ToList();
            }

            if (source.ObligatedWeeeSentOnList != null)
            {
                returnData.ObligatedWeeeSentOnData = source.ObligatedWeeeSentOnList.Select(n => new WeeeObligatedData(n.Id, new Aatf(n.WeeeSentOn.Aatf.Id, n.WeeeSentOn.Aatf.Name, n.WeeeSentOn.Aatf.ApprovalNumber, n.WeeeSentOn.Aatf.ComplianceYear), n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage) { WeeeSentOnId = n.WeeeSentOn.Id }).ToList();
            }

            if (source.ObligatedWeeeReusedList != null)
            {
                returnData.ObligatedWeeeReusedData = source.ObligatedWeeeReusedList.Select(n => new WeeeObligatedData(
                    n.Id,
                    null,
                    new Aatf(n.WeeeReused.Aatf.Id, n.WeeeReused.Aatf.Name, n.WeeeReused.Aatf.ApprovalNumber, n.WeeeReused.Aatf.ComplianceYear),
                    n.CategoryId,
                    n.NonHouseholdTonnage,
                    n.HouseholdTonnage)).ToList();
            }

            return returnData;
        }
    }
}
