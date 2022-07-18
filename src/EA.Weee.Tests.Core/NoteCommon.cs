namespace EA.Weee.Tests.Core
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
            Organisation recipientOrganisation = null,
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

            if (recipientOrganisation == null)
            {
                recipientOrganisation = Organisation.CreateRegisteredCompany("Test Organisation", "1234565");
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                database.WeeeContext.Schemes.Add(scheme);

                database.WeeeContext.SaveChanges();
            }

            if (startDate == null)
            {
                startDate = DateTime.UtcNow;
            }

            if (endDate == null)
            {
                endDate = DateTime.UtcNow;
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
                recipientOrganisation,
                startDate.Value,
                endDate.Value,
                wasteType,
                protocol,
                aatf,
                database.WeeeContext.GetCurrentUser(),
                noteTonnages)
            {
                ComplianceYear = complianceYear ?? (startDate.Value.Year)
            };

            return n;
        }

        public static Note CreateTransferNote(DatabaseWrapper database,
            Organisation organisation,
            Organisation recipientOrganisation = null,
            List<NoteTransferTonnage> noteTonnages = null,
            int? complianceYear = null)
        {
            if (organisation == null)
            {
                organisation = Organisation.CreateSoleTrader("Test Organisation");
            }

            if (recipientOrganisation == null)
            {
                recipientOrganisation = Organisation.CreateRegisteredCompany("Test Organisation", "1234565");
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                database.WeeeContext.Schemes.Add(scheme);

                database.WeeeContext.SaveChanges();
            }
        
            if (noteTonnages == null)
            {
                noteTonnages = new List<NoteTransferTonnage>();
            }

            if (!complianceYear.HasValue)
            {
                complianceYear = SystemTime.Now.Year;
            }

            return new Note(organisation,
                recipientOrganisation,
                database.WeeeContext.GetCurrentUser(),
                noteTonnages,
                complianceYear.Value,
                WasteType.HouseHold);
        }
    }
}
