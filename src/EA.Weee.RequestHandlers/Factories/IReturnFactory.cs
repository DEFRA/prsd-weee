namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;

    public interface IReturnFactory
    {
        Task<ReturnQuarter> GetReturnQuarter(Guid organisationId, FacilityType facilityType);
    }
}