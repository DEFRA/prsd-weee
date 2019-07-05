namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using Xunit;

    public class SpgAatfAeReturnDataCsvDataTests
    {
        [Fact]
        public async Task Execute_ReturnsDistinctAatfAeReturnData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);
                var organisation = helper.CreateOrganisation();

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 1);
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 1);

                await db.Model.SaveChangesAsync();

                var results = await db.StoredProcedures.SpgAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
            }
        }

        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataForQuarter()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);
                var organisation = helper.CreateOrganisation();

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2019"), db.WeeeContext.AatfContacts.First().Id, 1);
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 2);

                await db.Model.SaveChangesAsync();

                var results = await db.StoredProcedures.SpgAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
            }
        }

        [Fact]
        public async Task Execute_ReturnsAatfAeReturnDataIfDataExists()
        {            
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);
                var organisation = helper.CreateOrganisation();

                db.WeeeContext.AatfContacts.Add(ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()));
                db.WeeeContext.AatfAddress.Add(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db));

                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 1);
                CreateAatf(db, organisation, Convert.ToDateTime("01/02/2020"), db.WeeeContext.AatfContacts.First().Id, 2);
            
                await db.Model.SaveChangesAsync();

                var results = await db.StoredProcedures.SpgAatfAeReturnDataCsvData(2019, 4,
                        1, null, null, null, null);

                Assert.NotNull(results);
                Assert.Equal(2, results.Count);
            }
        }
        public void CreateAatf(DatabaseWrapper database, Organisation organisation, DateTime approvalDate, Guid contactId, int count)
        {
           var aatf = new AATF
            {
               Id = Guid.NewGuid(),
               Name = "aatfname" + count,
               CompetentAuthorityId = database.WeeeContext.UKCompetentAuthorities.First().Id,
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
