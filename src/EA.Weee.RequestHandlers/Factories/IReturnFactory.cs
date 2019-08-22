namespace EA.Weee.RequestHandlers.Factories
{
    using Core.AatfReturn;
    using Core.DataReturns;
    using System;
    using System.Threading.Tasks;

    public interface IReturnFactory
    {
        Task<Quarter> GetReturnQuarter(Guid organisationId, FacilityType facilityType);
    }
}