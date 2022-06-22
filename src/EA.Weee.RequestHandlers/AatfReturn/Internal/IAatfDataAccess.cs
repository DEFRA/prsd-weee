namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Evidence;

    public interface IAatfDataAccess
    {
        Task<Aatf> GetDetails(Guid id);

        Task UpdateDetails(Aatf oldDetails, Aatf newDetails);

        Task UpdateAddress(AatfAddress oldDetails, AatfAddress newDetails, Country country);

        Task<AatfContact> GetContact(Guid aatfId);

        Task UpdateContact(AatfContact oldDetails, AatfContactData newDetails, Country country);

        Task<bool> HasAatfData(Guid aatfId);

        Task<bool> HasAatfOrganisationOtherAeOrAatf(Aatf aatf);

        Task RemoveAatf(Guid aatfId);

        Task RemoveAatfData(Aatf aatf, IEnumerable<int> quarters);

        Task<List<short>> GetComplianceYearsForAatfByAatfId(Guid aatfId);

        Task<Guid> GetAatfByAatfIdAndComplianceYear(Guid aatfId, int complianceYear);

        Task<bool> IsLatestAatf(Guid id, Guid aatfId);
    }
}
