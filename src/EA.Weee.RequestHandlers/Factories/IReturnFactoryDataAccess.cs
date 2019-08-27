namespace EA.Weee.RequestHandlers.Factories
{
    using Domain.DataReturns;
    using System;
    using System.Threading.Tasks;

    public interface IReturnFactoryDataAccess
    {
        Task<bool> ValidateAatfApprovalDate(Guid organisationId, DateTime date, int year, EA.Weee.Core.AatfReturn.FacilityType facilityType);

        Task<bool> HasReturnQuarter(Guid organisationId, int year, QuarterType quarterType, EA.Weee.Core.AatfReturn.FacilityType facilityType);
    }
}