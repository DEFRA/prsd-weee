namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.DataReturns;

    public interface IReturnFactory
    {
        Task<Quarter> GetReturnQuarter(Guid organisationId, FacilityType facilityType);
    }
}