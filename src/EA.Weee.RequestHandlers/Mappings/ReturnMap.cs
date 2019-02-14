namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core;

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

            if (source.NonObligatedWeeeList != null)
            {
                returnData.NonObligatedData = source.NonObligatedWeeeList.Select(n => new NonObligatedData(n.CategoryId, n.Tonnage, n.Dcf)).ToList();
            }

            if (source.ObligatedWeeeList != null)
            {
                returnData.ObligatedData = source.ObligatedWeeeList.Select(n => new ObligatedData(n.WeeeReceived, n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage)).ToList();
            }

            return returnData;
        }
    }
}
