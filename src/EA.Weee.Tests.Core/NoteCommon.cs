namespace EA.Weee.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Model;
    using Prsd.Core.Helpers;
    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;

    public static class NoteCommon
    {
        public static Note CreateNote(DatabaseWrapper database, 
            Organisation organisation, 
            Scheme scheme, 
            Aatf aatf,
            WasteType wasteType = WasteType.HouseHold,
            Protocol protocol = Protocol.Actual,
            List<NoteTonnage> noteTonnages = null,
            Weee.Domain.Evidence.NoteType noteType = null)
        {
            if (organisation == null)
            {
                organisation = Organisation.CreateSoleTrader("Test Organisation");
            }

            if (scheme == null)
            {
                scheme = new Scheme(organisation);
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
                    DateTime.Now,
                    ObligatedWeeeIntegrationCommon.CreateDefaultContact(database.WeeeContext.Countries.First()), 
                    FacilityType.Aatf, (short)DateTime.Now.Year,
                    database.WeeeContext.LocalAreas.First(),
                    database.WeeeContext.PanAreas.First());
            }

            if (noteTonnages == null)
            {
                noteTonnages = new List<NoteTonnage>();
            }

            if (noteType == null)
            {
                noteType = NoteType.EvidenceNote;
            }

            return new Note(organisation,
                scheme,
                DateTime.Now,
                DateTime.Now,
                wasteType,
                protocol,
                aatf,
                noteType,
                database.WeeeContext.GetCurrentUser(),
                noteTonnages);
        }
    }
}
