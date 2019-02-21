namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Domain.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Aatf = Core.AatfReturn.AatfData;
    using Scheme = Core.AatfReturn.Scheme;

    public class ReturnMap : IMap<ReturnQuarterWindow, ReturnData>
    {
        public ReturnData Map(ReturnQuarterWindow source)
        {
            Guard.ArgumentNotNull(() => source, source);
            
            var returnData = new ReturnData()
            {
                Id = source.Return.Id,
                Quarter = new Quarter(source.Return.Quarter.Year, (QuarterType)source.Return.Quarter.Q),
                QuarterWindow = new QuarterWindow(source.QuarterWindow.StartDate, source.QuarterWindow.EndDate),
                ReturnOperatorData = new OperatorData(source.Return.OperatorId, source.Return.Operator.Organisation.OrganisationName, source.Return.Operator.Organisation.Id)
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
                returnData.NonObligatedData = source.NonObligatedWeeeList.Select(n => new NonObligatedData(n.CategoryId, n.Tonnage, n.Dcf)).ToList();
            }

            if (source.ObligatedWeeeReceivedList != null)
            {
                var aatf = new Aatf(
                    source.ObligatedWeeeReceivedList.Select(s => s.WeeeReceived.Aatf.Id).FirstOrDefault(),
                    source.ObligatedWeeeReceivedList.Select(s => s.WeeeReceived.Aatf.Name).FirstOrDefault(),
                    source.ObligatedWeeeReceivedList.Select(s => s.WeeeReceived.Aatf.ApprovalNumber).FirstOrDefault());
                var scheme = new Scheme(
                    source.ObligatedWeeeReceivedList.Select(s => s.WeeeReceived.Scheme.Id).FirstOrDefault(),
                    source.ObligatedWeeeReceivedList.Select(s => s.WeeeReceived.Scheme.SchemeName).FirstOrDefault());
                returnData.ObligatedWeeeReceivedData = source.ObligatedWeeeReceivedList.Select(n => new WeeeReceivedObligatedData(scheme, aatf, n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage)).ToList();
            }

            return returnData;
        }
    }
}
