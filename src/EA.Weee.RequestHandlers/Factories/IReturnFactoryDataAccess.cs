namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;

    public interface IReturnFactoryDataAccess
    {
        Task<IList<Aatf>> FetchAatfsByOrganisationFacilityTypeListAndYear(Guid organisationId, int year, EA.Weee.Core.AatfReturn.FacilityType facilityType);

        Task<bool> ValidateAatfApprovalDate(Guid organisationId, DateTime date, EA.Weee.Core.AatfReturn.FacilityType facilityType);

        Task<bool> ValidateReturnQuarter(Guid organisationId, int year, QuarterType quarterType, EA.Weee.Core.AatfReturn.FacilityType facilityType);
    }
}