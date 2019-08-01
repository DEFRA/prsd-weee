namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Lookup;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Xunit;
    using Contact = Domain.Organisation.Contact;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfAeReturnDataCsvDataTests
    {       
        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateExpectedToReport_RecordIsReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2019"), 1, "EA");

                db.WeeeContext.Aatfs.Add(aatf);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null, false);

                results.Count(x => x.AatfId == aatf.Id).Should().Be(1);
            }
        }

        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateThatIsNotExpectedToReport_RecordIsNotReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), 2, "EA");

                db.WeeeContext.Aatfs.Add(aatf);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 4,
                    1, null, null, null, null, false);

                results.Count(x => x.AatfId == aatf.Id).Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateExpectedToReportWithAuthorityParameter_CorrectRecordsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = CreateAatf(db, organisation, Convert.ToDateTime("01/01/2019"), 1, "EA");
                var aatf2 = CreateAatf(db, organisation, Convert.ToDateTime("01/01/2019"), 2, "EA");
                var aatf3 = CreateAatf(db, organisation, Convert.ToDateTime("01/01/2019"), 3, "NRW");
                var aatf4 = CreateAatf(db, organisation, Convert.ToDateTime("01/01/2019"), 4, "NRW");

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf2);
                db.WeeeContext.Aatfs.Add(aatf3);
                db.WeeeContext.Aatfs.Add(aatf4);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                        1, null, GetAuthority(db).Id, null, null, false);

                results.Count(x => x.AatfId == aatf1.Id).Should().Be(1);
                results.Count(x => x.AatfId == aatf2.Id).Should().Be(1);
                results.Count(x => x.AatfId == aatf3.Id).Should().Be(0);
                results.Count(x => x.AatfId == aatf4.Id).Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateExpectedToReportWithAreaParameter_CorrectRecordsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation1 = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = new Aatf("aatfname1", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());
                var aatf2 = new Aatf("aatfname2", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());
                var aatf3 = new Aatf("aatfname3", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, GetArea(db, "Wessex (WSX)"), db.WeeeContext.PanAreas.First());

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf2);
                db.WeeeContext.Aatfs.Add(aatf3);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                        1, null, GetAuthority(db).Id, GetArea(db, "Wessex (WSX)").Id, null, false);

                results.Count(x => x.AatfId == aatf1.Id).Should().Be(0);
                results.Count(x => x.AatfId == aatf2.Id).Should().Be(0);
                results.Count(x => x.AatfId == aatf3.Id).Should().Be(1);
            }
        }

        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateExpectedToReportWithPanAreaParameter_CorrectRecordsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation1 = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = new Aatf("aatfname1", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), GetPanArea(db, "North"));
                var aatf2 = new Aatf("aatfname2", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), GetPanArea(db, "North"));
                var aatf3 = new Aatf("aatfname3", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), GetPanArea(db, "Midlands"));

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf2);
                db.WeeeContext.Aatfs.Add(aatf3);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                        1, null, null, null, GetPanArea(db, "North").Id, false);

                results.Count(x => x.AatfId == aatf1.Id).Should().Be(1);
                results.Count(x => x.AatfId == aatf2.Id).Should().Be(1);
                results.Count(x => x.AatfId == aatf3.Id).Should().Be(0);
            }
        }

        public Aatf CreateAatf(DatabaseWrapper database, Organisation organisation, DateTime approvalDate, int count, string cA)
        {
            var aatfContact = ObligatedWeeeIntegrationCommon.CreateDefaultContact(database.WeeeContext.Countries.First());
            var aatfAddress = ObligatedWeeeIntegrationCommon.CreateAatfAddress(database);
            var aatf = new Aatf("aatfname" + count, database.WeeeContext.UKCompetentAuthorities.First(c => c.Abbreviation == cA), "number", AatfStatus.Approved, organisation, aatfAddress, AatfSize.Large, approvalDate, aatfContact, FacilityType.Aatf, 2019, database.WeeeContext.LocalAreas.First(), database.WeeeContext.PanAreas.First());

            return aatf;
        }

        private UKCompetentAuthority GetAuthority(DatabaseWrapper db)
        {
            return db.WeeeContext.UKCompetentAuthorities.First(x => x.Abbreviation == "EA");
        }

        private PanArea GetPanArea(DatabaseWrapper db, string name)
        {
            return db.WeeeContext.PanAreas.First(x => x.Name == name);
        }

        private static LocalArea GetArea(DatabaseWrapper db, string name)
        {
            return db.WeeeContext.LocalAreas.First(x => x.Name == name);
        }
    }    
}
