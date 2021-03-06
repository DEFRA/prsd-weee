﻿namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using Domain.AatfReturn;
    using System;
    using System.Threading.Tasks;

    public interface IGetOrganisationDeletionStatus
    {
        Task<CanOrganisationBeDeletedFlags> Validate(Guid organisationId, int year, FacilityType facilityType);
    }
}