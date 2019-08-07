namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Scheme;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Prsd.Core;
    using Xunit;
    using Contact = Domain.Organisation.Contact;
    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;

    public class GetAatfAeReturnDataCsvDataTests
    {
        private readonly Fixture fixture;

        public GetAatfAeReturnDataCsvDataTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateExpectedToReport_RecordIsReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf = CreateAatf(db, organisation, Convert.ToDateTime("01/02/2019"), 1, "EA");

                db.WeeeContext.Aatfs.Add(aatf);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                        1, null, null, null, null, false);

                results.Count(x => x.AatfId == aatf.Id).Should().Be(1);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndReturnNotStarted_RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                db.WeeeContext.Aatfs.Add(aatf);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, false);

                var record = results.First(x => x.AatfId == aatf.Id);
                record.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                record.CompetentAuthorityAbbr.Should().Be("EA");
                record.Name.Should().Be(aatf.Name);
                record.OrganisationName.Should().Be(organisation.OrganisationName);
                record.CreatedDate.Should().BeNull();
                record.ReturnStatus.Should().Be("Not Started");
                record.SubmittedBy.Should().BeNullOrWhiteSpace();
                record.SubmittedDate.Should().BeNull();
                record.ReSubmission.Should().Be("First submission");
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndResubmissionCreatedAndResubmissionsRequested_RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var date = new DateTime(2019, 1, 1, 11, 10, 1);
                SystemTime.Freeze(date);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);

                SystemTime.Unfreeze();

                SystemTime.Freeze(date.Date.AddDays(1));
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);
                SystemTime.Unfreeze();

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.Returns.Add(@return2);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, true);

                var initialSubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(0);
                
                initialSubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                initialSubmission.CompetentAuthorityAbbr.Should().Be("EA");
                initialSubmission.Name.Should().Be(aatf.Name);
                initialSubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                initialSubmission.CreatedDate.Should().Be(date);
                initialSubmission.ReturnStatus.Should().Be("Submitted");
                initialSubmission.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                initialSubmission.SubmittedDate.Should().Be(date);
                initialSubmission.ReSubmission.Should().Be("First submission");

                var resubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(1);
                
                resubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                resubmission.CompetentAuthorityAbbr.Should().Be("EA");
                resubmission.Name.Should().Be(aatf.Name);
                resubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                resubmission.CreatedDate.Should().Be(date.Date.AddDays(1));
                resubmission.ReturnStatus.Should().Be("Started");
                resubmission.SubmittedBy.Should().BeNullOrWhiteSpace();
                resubmission.SubmittedDate.Should().BeNull();
                resubmission.ReSubmission.Should().Be("Resubmission");
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndResubmissionCreatedAndResubmissionsRequestedWithReturnsWithVarietyOfData_RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var date = new DateTime(2019, 1, 1, 11, 10, 1);
                SystemTime.Freeze(date);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                db.WeeeContext.WeeeSentOn.Add(new WeeeSentOn(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db),
                    ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return));
                db.WeeeContext.WeeeReceived.Add(new WeeeReceived(new Scheme(organisation), aatf, @return));
                db.WeeeContext.WeeeReused.Add(new WeeeReused(aatf, @return));

                @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                db.WeeeContext.WeeeSentOn.Add(new WeeeSentOn(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db),
                    ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return2));
                db.WeeeContext.WeeeReceived.Add(new WeeeReceived(new Scheme(organisation), aatf, @return2));
                db.WeeeContext.WeeeReused.Add(new WeeeReused(aatf, @return2));

                @return2.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.Returns.Add(@return2);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, true);

                var initialSubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(0);
                initialSubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                initialSubmission.CompetentAuthorityAbbr.Should().Be("EA");
                initialSubmission.Name.Should().Be(aatf.Name);
                initialSubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                initialSubmission.CreatedDate.Should().Be(date);
                initialSubmission.ReturnStatus.Should().Be("Submitted");
                initialSubmission.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                initialSubmission.SubmittedDate.Should().Be(date);
                initialSubmission.ReSubmission.Should().Be("First submission");

                var resubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(1);
                resubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                resubmission.CompetentAuthorityAbbr.Should().Be("EA");
                resubmission.Name.Should().Be(aatf.Name);
                resubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                resubmission.CreatedDate.Should().Be(date);
                resubmission.ReturnStatus.Should().Be("Submitted");
                resubmission.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                resubmission.SubmittedDate.Should().Be(date);
                resubmission.ReSubmission.Should().Be("Resubmission");

                SystemTime.Unfreeze();
            }
        }
        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndResubmissionSubmittedAndResubmissionsRequested_RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                var date1 = new DateTime(2019, 1, 1, 11, 10, 1);
                SystemTime.Freeze(date1);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);
                @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
                SystemTime.Unfreeze();

                var date2 = new DateTime(2019, 1, 2, 11, 10, 1);
                SystemTime.Freeze(date2);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);
                @return2.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
                SystemTime.Unfreeze();

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.Returns.Add(@return2);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return2));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, true);

                var initialSubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(0);
                initialSubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                initialSubmission.CompetentAuthorityAbbr.Should().Be("EA");
                initialSubmission.Name.Should().Be(aatf.Name);
                initialSubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                initialSubmission.CreatedDate.Should().Be(date1);
                initialSubmission.ReturnStatus.Should().Be("Submitted");
                initialSubmission.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                initialSubmission.SubmittedDate.Should().Be(date1);
                initialSubmission.ReSubmission.Should().Be("First submission");

                var resubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(1);
                resubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                resubmission.CompetentAuthorityAbbr.Should().Be("EA");
                resubmission.Name.Should().Be(aatf.Name);
                resubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                resubmission.CreatedDate.Should().Be(date2);
                resubmission.ReturnStatus.Should().Be("Submitted");
                resubmission.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                resubmission.SubmittedDate.Should().Be(date2);
                resubmission.ReSubmission.Should().Be("Resubmission");

                SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndResubmissionCreatedAndResubmissionsNotRequested__RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var date = new DateTime(2019, 1, 1, 11, 10, 1);
                SystemTime.Freeze(date);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return2);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, false);

                var initialSubmission = results.Where(x => x.AatfId == aatf.Id).ElementAt(0);
                initialSubmission.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                initialSubmission.CompetentAuthorityAbbr.Should().Be("EA");
                initialSubmission.Name.Should().Be(aatf.Name);
                initialSubmission.OrganisationName.Should().Be(organisation.OrganisationName);
                initialSubmission.CreatedDate.Should().Be(date);
                initialSubmission.ReturnStatus.Should().Be("Submitted");
                initialSubmission.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                initialSubmission.SubmittedDate.Should().Be(date);
                initialSubmission.ReSubmission.Should().Be("First submission");

                SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndReturnStarted_RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var date = new DateTime(2019, 1, 1, 11, 10, 1);
                SystemTime.Freeze(date);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.Aatfs.Add(aatf);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, false);

                var record = results.First(x => x.AatfId == aatf.Id);
                record.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                record.CompetentAuthorityAbbr.Should().Be("EA");
                record.Name.Should().Be(aatf.Name);
                record.OrganisationName.Should().Be(organisation.OrganisationName);
                record.CreatedDate.Should().Be(date);
                record.ReturnStatus.Should().Be("Started");
                record.SubmittedBy.Should().BeNullOrWhiteSpace();
                record.SubmittedDate.Should().BeNull();
                record.ReSubmission.Should().Be("First submission");

                SystemTime.Unfreeze();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovalDateExpectedToReportAndReturnSubmitted_RecordDataIsAsExpected()
        {
            using (var db = new DatabaseWrapper())
            {
                var date = new DateTime(2019, 1, 1, 11, 10, 1);
                SystemTime.Freeze(date);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = new Aatf(fixture.Create<string>(), GetAuthority(db), fixture.Create<string>().Substring(0, 10), AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), db.WeeeContext.PanAreas.First());

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019,
                    QuarterType.Q1);

                @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                    1, null, null, null, null, false);

                var record = results.First(x => x.AatfId == aatf.Id);
                record.ApprovalNumber.Should().Be(aatf.ApprovalNumber);
                record.CompetentAuthorityAbbr.Should().Be("EA");
                record.Name.Should().Be(aatf.Name);
                record.OrganisationName.Should().Be(organisation.OrganisationName);
                record.CreatedDate.Should().Be(date);
                record.ReturnStatus.Should().Be("Submitted");
                record.SubmittedBy.Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
                record.SubmittedDate.Should().Be(date);
                record.ReSubmission.Should().Be("First submission");

                SystemTime.Unfreeze();
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

        [Fact]
        public async Task Execute_GivenAaatfWithApprovalDateExpectedToReportWithFacilityParameter_CorrectRecordsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var organisation1 = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = new Aatf("aatfname1", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), GetPanArea(db, "North"));
                var aatf2 = new Aatf("aatfname2", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, db.WeeeContext.LocalAreas.First(), GetPanArea(db, "North"));
                var ae = new Aatf("aatfname3", GetAuthority(db), "number", AatfStatus.Approved, organisation1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), AatfSize.Large, Convert.ToDateTime("01/02/2019"), ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Ae, 2019, db.WeeeContext.LocalAreas.First(), GetPanArea(db, "North"));

                db.WeeeContext.Aatfs.Add(aatf1);
                db.WeeeContext.Aatfs.Add(aatf2);
                db.WeeeContext.Aatfs.Add(ae);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeReturnDataCsvData(2019, 1,
                        1, null, null, null, null, false);

                results.Count(x => x.AatfId == aatf1.Id).Should().Be(1);
                results.Count(x => x.AatfId == aatf2.Id).Should().Be(1);
                results.Count(x => x.AatfId == ae.Id).Should().Be(0);
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
