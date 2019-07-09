namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using Xunit;

    public class GetAatfAeReturnDataCsvDataTests
    {       
        //[Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForQuarter()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);
                var organisation = helper.CreateOrganisation();

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2019"), db.WeeeContext.AatfContacts.First().Id, 1, "EA");
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 2, "EA");

                await db.Model.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
            }
        }

        //[Fact]
        public async Task Execute_ReturnsAatfAeReturnDataIfDataExists()
        {            
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);
                var organisation = helper.CreateOrganisation();

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 1, "EA");
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 2, "EA");
            
                await db.Model.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                Assert.Equal(2, results.Count);
            }
        }

        //[Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForCA()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);
                var organisation = helper.CreateOrganisation();

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 1, "EA");
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 2, "EA");
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 3, "NRW");
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 4, "NRW");

                await db.Model.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, db.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == "EA").First().Id, null, null);

                Assert.NotNull(results);
                Assert.Equal(2, results.Count);
            }
        }

        //[Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForEAArea()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
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
                Assert.Equal(1, results.Count);
            }
        }

        //[Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForEAPanArea()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
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
                Assert.Equal(2, results.Count);
            }
        }

        public void CreateAatf(DatabaseWrapper database, Organisation organisation, DateTime approvalDate, Guid contactId, int count, string cA)
        {
           var aatf = new AATF
            {
               Id = Guid.NewGuid(),
               Name = "aatfname" + count,
               CompetentAuthorityId = database.WeeeContext.UKCompetentAuthorities.Where(x => x.Abbreviation == cA).First().Id,
               ApprovalNumber = "number",
               Status = 1,
               Organisation = organisation,
               SiteAddressId = database.WeeeContext.AatfAddress.First().Id,
               Size = 2,
               ApprovalDate = approvalDate,
               ContactId = contactId,
               FacilityType = 1
            };
            
            database.Model.AATFs.Add(aatf);            
        }
    }    
}
