namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;

    public interface IReturnFactoryDataAccess
    {
        Task<bool> ValidateAatfApprovalDate(Guid organisationId, DateTime date, int year, EA.Weee.Core.AatfReturn.FacilityType facilityType);

        Task<bool> HasReturnQuarter(Guid organisationId, int year, QuarterType quarterType, EA.Weee.Core.AatfReturn.FacilityType facilityType);
    }
}