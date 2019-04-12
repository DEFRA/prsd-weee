﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Aatf = Core.AatfReturn.AatfData;

    public class ReturnMap : IMap<ReturnQuarterWindow, ReturnData>
    {
        private readonly IMapper mapper;

        public ReturnMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReturnData Map(ReturnQuarterWindow source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var returnData = new ReturnData()
            {
                Id = source.Return.Id,
                Quarter = new Quarter(source.Return.Quarter.Year, (QuarterType)source.Return.Quarter.Q),
                QuarterWindow = new QuarterWindow(source.QuarterWindow.StartDate, source.QuarterWindow.EndDate),
                ReturnOperatorData = new OperatorData(source.Return.OperatorId, source.Return.Operator.Organisation.OrganisationName, source.Return.Operator.Organisation.Id),
                SchemeDataItems = source.ReturnSchemes.Select(s => mapper.Map<EA.Weee.Domain.Scheme.Scheme, SchemeData>(s.Scheme)).ToList() // CREATE UNIT TEST TO TEST THIS PROPERTY IS SET AND MAPPED
            };

            if (source.Aatfs != null)
            {
                var aatfReturnList = new List<Aatf>();

                foreach (var sourceAatf in source.Aatfs)
                {
                    aatfReturnList.Add(new Aatf(sourceAatf.Id, sourceAatf.Name, sourceAatf.ApprovalNumber));
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
                    new Aatf(n.WeeeReceived.Aatf.Id, n.WeeeReceived.Aatf.Name, n.WeeeReceived.Aatf.ApprovalNumber),
                    n.CategoryId,
                    n.NonHouseholdTonnage,
                    n.HouseholdTonnage)).ToList();
            }

            if (source.ObligatedWeeeReusedList != null)
            {
                returnData.ObligatedWeeeReusedData = source.ObligatedWeeeReusedList.Select(n => new WeeeObligatedData(
                    n.Id,
                    null,
                    new Aatf(n.WeeeReused.Aatf.Id, n.WeeeReused.Aatf.Name, n.WeeeReused.Aatf.ApprovalNumber),
                    n.CategoryId,
                    n.NonHouseholdTonnage,
                    n.HouseholdTonnage)).ToList();
            }

            return returnData;
        }
    }
}
