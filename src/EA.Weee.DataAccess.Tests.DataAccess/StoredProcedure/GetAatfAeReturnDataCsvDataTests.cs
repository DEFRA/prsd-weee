namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using Xunit;
    using Contact = Domain.Organisation.Contact;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfAeReturnDataCsvDataTests
    {       
        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForQuarter()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                var aatf1 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2019"), 1, "EA");
                var aatf2 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 2, "EA");

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf1);
                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                results.Where(x => x.AatfId == aatf1.Id).Count().Equals(1);
                results.Where(x => x.AatfId == aatf2.Id).Count().Equals(1);
            }
        }

        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataIfDataExists()
        {            
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

               var aatf1 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 1, "EA");
               var aatf2 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 2, "EA");

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                results.Where(x => x.AatfId == aatf1.Id).Count().Equals(1);
                results.Where(x => x.AatfId == aatf2.Id).Count().Equals(1);
            }
        }

        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForCA()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 1, "EA");
                var aatf2 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 2, "EA");
                var aatf3 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 3, "NRW");
                var aatf4 = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 4, "NRW");

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First().Id, null, null);

                Assert.NotNull(results);
                results.Where(x => x.AatfId == aatf1.Id).Count().Equals(1);
                results.Where(x => x.AatfId == aatf2.Id).Count().Equals(1);
                results.Where(x => x.AatfId == aatf3.Id).Count().Equals(0);
                results.Where(x => x.AatfId == aatf4.Id).Count().Equals(0);
            }
        }

        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForEAArea()
        {
            using (var db = new DatabaseWrapper())
            {
                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                var organisation1 = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                var aatf1 = new Aatf("aatfname1", db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First(), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2020"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());
                var aatf2 = new Aatf("aatfname2", db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First(), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2020"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());
                var aatf3 = new Aatf("aatfname3", db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First(), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2020"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.Where(x => x.Name == "Wessex (WSX)").First(), db.WeeeContext.PanAreas.First());

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf2);
                db.WeeeContext.Aatfs.Add(aatf3);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First().Id, db.WeeeContext.LocalAreas.Where(x => x.Name == "Wessex (WSX)").First().Id, null);

                Assert.NotNull(results);
                results.Where(x => x.AatfId == aatf1.Id).Count().Equals(0);
                results.Where(x => x.AatfId == aatf3.Id).Count().Equals(1);
            }
        }

        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForEAPanArea()
        {
            using (var db = new DatabaseWrapper())
            {
                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                var organisation1 = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                var aatf1 = new Aatf("aatfname1", db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First(), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2020"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.Where(x => x.Name == "North").First());
                var aatf2 = new Aatf("aatfname2", db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First(), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2020"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.Where(x => x.Name == "North").First());
                var aatf3 = new Aatf("aatfname3", db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First(), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2020"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.Where(x => x.Name == "Midlands").First());

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf2);
                db.WeeeContext.Aatfs.Add(aatf3);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First().Id, null, db.WeeeContext.PanAreas.Where(x => x.Name == "North").First().Id);

                Assert.NotNull(results);
                results.Where(x => x.AatfId == aatf1.Id).Count().Equals(1);
                results.Where(x => x.AatfId == aatf2.Id).Count().Equals(1);
                results.Where(x => x.AatfId == aatf3.Id).Count().Equals(0);
            }
        }

        public Aatf CreateAatf(DatabaseWrapper database, Organisation organisation, DateTime approvalDate, int count, string cA)
        {
            var aatfContact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(database.WeeeContext.Countries.First());
            var aatfAddress = ObligatedWeeeIntegrationCommon.CreateAatfAddress(database);
            var aatf = new Aatf("aatfname" + count, database.WeeeContext.UKCompetentAuthorities.First(c => c.Abbreviation == cA), "number", AatfStatus.Approved, organisation, aatfAddress, AatfSize.Large, approvalDate, aatfContact, FacilityType.Aatf, 2019, database.WeeeContext.LocalAreas.First(), database.WeeeContext.PanAreas.First());

            return aatf;
        }
    }    
}
