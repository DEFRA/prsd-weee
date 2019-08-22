﻿namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    public interface IAatfDataAccess
    {
        Task<Aatf> GetDetails(Guid id);

        Task UpdateDetails(Aatf oldDetails, Aatf newDetails);

        Task UpdateAddress(AatfAddress oldDetails, AatfAddress newDetails, Country country);

        Task<AatfContact> GetContact(Guid aatfId);

        Task UpdateContact(AatfContact oldDetails, AatfContactData newDetails, Country country);

        Task<bool> HasAatfData(Guid aatfId);

        Task<bool> HasAatfOrganisationOtherAeOrAatf(Aatf aatf);

        Task<bool> HasAatfOrganisationOtherAeOrAatfWithQuarterWindow(Aatf aatf, EA.Weee.Domain.DataReturns.QuarterWindow quarterWindow);

        Task RemoveAatf(Guid aatfId);

        Task RemoveAatfData(Aatf aatf, IEnumerable<int> quarters, CanApprovalDateBeChangedFlags flags);

        Task<List<short>> GetComplianceYearsForAatfByAatfId(Guid aatfId);

        Task<Guid> GetAatfId(Guid aatfId, short complianceYear);
    }
}
