﻿namespace EA.Weee.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using EA.Prsd.Core;
    using Model;
    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;

    public static class NoteCommon
    {
        public static Note CreateNote(DatabaseWrapper database, 
            Organisation organisation = null, 
            Scheme scheme = null, 
            Aatf aatf = null,
            WasteType wasteType = WasteType.HouseHold,
            Protocol protocol = Protocol.Actual,
            List<NoteTonnage> noteTonnages = null,
            DateTime? startDate = null,
            DateTime? endDate = null, 
            int? complianceYear = null)
        {
            if (organisation == null)
            {
                organisation = Organisation.CreateSoleTrader("Test Organisation");
            }

            if (scheme == null)
            {
                scheme = new Scheme(organisation);
            }

            if (startDate == null)
            {
                startDate = DateTime.Now;
            }

            if (endDate == null)
            {
                endDate = DateTime.Now;
            }

            if (aatf == null)
            {
                aatf = new Aatf("aatfName", 
                    database.WeeeContext.UKCompetentAuthorities.First(), 
                    "number", 
                    AatfStatus.Approved, 
                    organisation, 
                    ObligatedWeeeIntegrationCommon.CreateAatfAddress(database), 
                    AatfSize.Large,
                    startDate.Value,
                    ObligatedWeeeIntegrationCommon.CreateDefaultContact(database.WeeeContext.Countries.First()), 
                    FacilityType.Aatf, 
                    startDate.Value.Year,
                    database.WeeeContext.LocalAreas.First(),
                    database.WeeeContext.PanAreas.First());
            }

            if (noteTonnages == null)
            {
                noteTonnages = new List<NoteTonnage>();
            }

            Note n = new Note(organisation,
                scheme,
                startDate.Value,
                endDate.Value,
                wasteType,
                protocol,
                aatf,
                database.WeeeContext.GetCurrentUser(),
                noteTonnages);

            n.ComplianceYear = complianceYear.HasValue ? complianceYear.Value : startDate.HasValue ? startDate.Value.Year : SystemTime.UtcNow.Year;

            return n;
        }

        public static Note CreateTransferNote(DatabaseWrapper database,
            Organisation organisation,
            Scheme scheme,
            List<NoteTransferTonnage> noteTonnages = null,
            List<NoteTransferCategory> noteTransferCategories = null,
            int? complianceYear = null)
        {
            if (organisation == null)
            {
                organisation = Organisation.CreateSoleTrader("Test Organisation");
            }

            if (scheme == null)
            {
                scheme = new Scheme(organisation);
            }

            if (noteTonnages == null)
            {
                noteTonnages = new List<NoteTransferTonnage>();
            }

            if (noteTransferCategories == null)
            {
                noteTransferCategories = new List<NoteTransferCategory>();
            }

            if (!complianceYear.HasValue)
            {
                complianceYear = SystemTime.Now.Year;
            }

            return new Note(organisation,
                scheme,
                database.WeeeContext.GetCurrentUser(),
                noteTonnages,
                noteTransferCategories,
                complianceYear.Value,
                WasteType.HouseHold);
        }
    }
}
