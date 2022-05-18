namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.DeleteAatf
{
    using AutoFixture;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Factories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class AatfDataAccessTests
    {
        private readonly Fixture fixture;
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public AatfDataAccessTests()
        {
            fixture = new Fixture();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
        }

        [Fact]
        public async void RemoveAatf_GivenAatf_AatfShouldShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatfDoNotDelete = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);
                databaseWrapper.WeeeContext.Aatfs.Add(aatfDoNotDelete);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatf.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatfDoNotDelete.Id).Should().NotBeEmpty();

                await aatfDataAccess.RemoveAatf(aatf.Id);

                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatf.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatfDoNotDelete.Id).Should().NotBeEmpty();
            }
        }

        [Fact]
        public async void RemoveAatf_GivenAatfWithReturnAatfEntries_AatfShouldShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var aatfDoNotDelete = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatfDoNotDelete, @return));
                databaseWrapper.WeeeContext.Aatfs.Add(aatf);
                databaseWrapper.WeeeContext.Aatfs.Add(aatfDoNotDelete);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatf.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatfDoNotDelete.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.ReturnAatfs.Where(o => o.Aatf.Id == aatf.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.ReturnAatfs.Where(o => o.Aatf.Id == aatfDoNotDelete.Id).Should().NotBeEmpty();

                await aatfDataAccess.RemoveAatf(aatf.Id);

                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatf.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.Aatfs.Where(o => o.Id == aatfDoNotDelete.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.ReturnAatfs.Where(o => o.Aatf.Id == aatf.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.ReturnAatfs.Where(o => o.Aatf.Id == aatfDoNotDelete.Id).Should().NotBeEmpty();
            }
        }

        private GenericDataAccess GetGenericDataAccess(DatabaseWrapper databaseWrapper)
        {
            return new GenericDataAccess(databaseWrapper.WeeeContext);
        }

        [Fact]
        public async void RemoveAatf_GivenAatfWithSentOnData_ExceptionExpected()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, null);

                AddWeeeSentOn(databaseWrapper, aatf, @return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await Xunit.Record.ExceptionAsync(() => aatfDataAccess.RemoveAatf(aatf.Id));

                result.Should().BeOfType<InvalidOperationException>();
            }
        }

        [Fact]
        public async void RemoveAatf_GivenAatfWithReusedData_ExceptionExpected()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, null);

                AddWeeeReused(databaseWrapper, aatf, @return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await Xunit.Record.ExceptionAsync(() => aatfDataAccess.RemoveAatf(aatf.Id));

                result.Should().BeOfType<InvalidOperationException>();
            }
        }

        [Fact]
        public async void RemoveAatf_GivenAatfWithReceivedData_ExceptionExpected()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, null);

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await Xunit.Record.ExceptionAsync(() => aatfDataAccess.RemoveAatf(aatf.Id));

                result.Should().BeOfType<InvalidOperationException>();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterWithNonObligatedData_AatfDataAndStartedReturnShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q2);

                databaseWrapper.WeeeContext.NonObligatedWeee.Add(new Domain.AatfReturn.NonObligatedWeee(@return, 1, true, 1));
                databaseWrapper.WeeeContext.NonObligatedWeee.Add(new Domain.AatfReturn.NonObligatedWeee(@return, 1, false, 1));
                databaseWrapper.WeeeContext.NonObligatedWeee.Add(new Domain.AatfReturn.NonObligatedWeee(@return2, 1, true, 1));
                databaseWrapper.WeeeContext.NonObligatedWeee.Add(new Domain.AatfReturn.NonObligatedWeee(@return2, 1, false, 1));

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.NonObligatedWeee.Count(n => n.ReturnId == @return.Id).Should().Be(2);
                databaseWrapper.WeeeContext.NonObligatedWeee.Count(n => n.ReturnId == @return2.Id).Should().Be(2);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.NonObligatedWeee.Count(n => n.ReturnId == @return.Id).Should().Be(0);
                databaseWrapper.WeeeContext.NonObligatedWeee.Count(n => n.ReturnId == @return2.Id).Should().Be(2);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAeAndSingleQuarter_AeReturnShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var ae = ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Ae, 2019, QuarterType.Q1);

                SetupQuarterWindow(ae.ComplianceYear, QuarterType.Q1);

                databaseWrapper.WeeeContext.Aatfs.Add(ae);
                databaseWrapper.WeeeContext.Returns.Add(@return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);

                await aatfDataAccess.RemoveAatfData(ae, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(0);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAeAndReturnInDifferentQuarter_OtherAeReturnShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var ae = ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Ae, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Ae, 2019, QuarterType.Q2);

                SetupQuarterWindow(ae.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(ae.ComplianceYear, QuarterType.Q2);

                databaseWrapper.WeeeContext.Aatfs.Add(ae);
                databaseWrapper.WeeeContext.Returns.Add(@return);
                databaseWrapper.WeeeContext.Returns.Add(return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);
                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == return2.Id).Should().Be(1);

                await aatfDataAccess.RemoveAatfData(ae, new List<int>() { 2 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);
                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == return2.Id).Should().Be(0);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterAndOrganisationHasAeReturn_AeReturnShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var nonAatfReturn = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Ae, 2019, QuarterType.Q1);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);

                databaseWrapper.WeeeContext.NonObligatedWeee.Add(new Domain.AatfReturn.NonObligatedWeee(@return, 1, true, 1));
                databaseWrapper.WeeeContext.NonObligatedWeee.Add(new Domain.AatfReturn.NonObligatedWeee(@return, 1, false, 1));

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);
                databaseWrapper.WeeeContext.Returns.Add(nonAatfReturn);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);
                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == nonAatfReturn.Id).Should().Be(1);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(0);
                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == nonAatfReturn.Id).Should().Be(1);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarter_AatfDataAndStartedReturnShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);

                var received = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var reused = AddWeeeReused(databaseWrapper, aatf, @return);
                var sentOn = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, reused);
                AddReusedAmount(databaseWrapper, reused);
                AddWeeeSentOnAmount(databaseWrapper, sentOn);
                AddWeeeReceivedAmount(databaseWrapper, received);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, received, sentOn, reused, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllFalse(databaseWrapper, @return, received, sentOn, reused, aatf);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterWithReturnInSameQuarterButDifferentYear_OtherYearReturnsShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2020, QuarterType.Q1);

                SetupQuarterWindow(2019, QuarterType.Q1);
                SetupQuarterWindow(2020, QuarterType.Q1);

                var received = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var reused = AddWeeeReused(databaseWrapper, aatf, @return);
                var sentOn = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, reused);
                AddReusedAmount(databaseWrapper, reused);
                AddWeeeSentOnAmount(databaseWrapper, sentOn);
                AddWeeeReceivedAmount(databaseWrapper, received);

                var received2 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                var reused2 = AddWeeeReused(databaseWrapper, aatf, @return2);
                var sentOn2 = AddWeeeSentOn(databaseWrapper, aatf, @return2);
                AddReturnAatf(databaseWrapper, aatf, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, reused2);
                AddReusedAmount(databaseWrapper, reused2);
                AddWeeeSentOnAmount(databaseWrapper, sentOn2);
                AddWeeeReceivedAmount(databaseWrapper, received2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllFalse(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterWithReturnInSameYearButDifferentQuarter_OtherYearReturnsShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q2);

                var received = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var reused = AddWeeeReused(databaseWrapper, aatf, @return);
                var sentOn = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, reused);
                AddReusedAmount(databaseWrapper, reused);
                AddWeeeSentOnAmount(databaseWrapper, sentOn);
                AddWeeeReceivedAmount(databaseWrapper, received);

                var received2 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                var reused2 = AddWeeeReused(databaseWrapper, aatf, @return2);
                var sentOn2 = AddWeeeSentOn(databaseWrapper, aatf, @return2);
                AddReturnAatf(databaseWrapper, aatf, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, reused2);
                AddReusedAmount(databaseWrapper, reused2);
                AddWeeeSentOnAmount(databaseWrapper, sentOn2);
                AddWeeeReceivedAmount(databaseWrapper, received2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllFalse(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterWithResubmissions_ReturnAndResubmissionsShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);

                var received = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var reused = AddWeeeReused(databaseWrapper, aatf, @return);
                var sentOn = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, reused);
                AddReusedAmount(databaseWrapper, reused);
                AddWeeeSentOnAmount(databaseWrapper, sentOn);
                AddWeeeReceivedAmount(databaseWrapper, received);

                var received2 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                var reused2 = AddWeeeReused(databaseWrapper, aatf, @return2);
                var sentOn2 = AddWeeeSentOn(databaseWrapper, aatf, @return2);
                AddReturnAatf(databaseWrapper, aatf, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, reused2);
                AddReusedAmount(databaseWrapper, reused2);
                AddWeeeSentOnAmount(databaseWrapper, sentOn2);
                AddWeeeReceivedAmount(databaseWrapper, received2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                @return2.ParentId = @return.Id;
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllFalse(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllFalse(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndMultipleQuarter_AatfDataAndStartedReturnShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q2);

                var received = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var reused = AddWeeeReused(databaseWrapper, aatf, @return);
                var sentOn = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, reused);
                AddReusedAmount(databaseWrapper, reused);
                AddWeeeSentOnAmount(databaseWrapper, sentOn);
                AddWeeeReceivedAmount(databaseWrapper, received);

                var received2 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                var reused2 = AddWeeeReused(databaseWrapper, aatf, @return2);
                var sentOn2 = AddWeeeSentOn(databaseWrapper, aatf, @return2);
                AddReturnAatf(databaseWrapper, aatf, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, reused2);
                AddReusedAmount(databaseWrapper, reused2);
                AddWeeeSentOnAmount(databaseWrapper, sentOn2);
                AddWeeeReceivedAmount(databaseWrapper, received2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1, 2 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllFalse(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllFalse(databaseWrapper, @return2, received2, sentOn2, reused2, aatf);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndSingleQuarter_AatfDataShouldBeRemovedAndReturnNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(aatf2.ComplianceYear, QuarterType.Q1);

                var received = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var reused = AddWeeeReused(databaseWrapper, aatf, @return);
                var sentOn = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, reused);
                AddReusedAmount(databaseWrapper, reused);
                AddWeeeSentOnAmount(databaseWrapper, sentOn);
                AddWeeeReceivedAmount(databaseWrapper, received);

                var received2 = AddWeeeReceived(databaseWrapper, organisation, aatf2, @return);
                var reused2 = AddWeeeReused(databaseWrapper, aatf2, @return);
                var sentOn2 = AddWeeeSentOn(databaseWrapper, aatf2, @return);
                AddReusedSite(databaseWrapper, reused2);
                AddReusedAmount(databaseWrapper, reused2);
                AddWeeeSentOnAmount(databaseWrapper, sentOn2);
                AddWeeeReceivedAmount(databaseWrapper, received2);

                databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf2, @return));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, received, sentOn, reused, aatf);
                AssertAllTrue(databaseWrapper, @return, received2, sentOn2, reused2, aatf2);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf2.Id).Should().BeTrue();

                AssertAllAatfDataTrue(databaseWrapper, @return, aatf2, received2, reused2, sentOn2);

                AssertAllAatfDataFalse(databaseWrapper, @return, aatf, received, reused, sentOn);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndReturnsInDifferentQuarter_AatfDataShouldBeRemovedAndOtherReturnDataNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q2);

                var aatf1ReceivedReturn1 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var aatf1ReusedReturn1 = AddWeeeReused(databaseWrapper, aatf, @return);
                var aatf1SentOnReturn1 = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, aatf1ReusedReturn1);
                AddReusedAmount(databaseWrapper, aatf1ReusedReturn1);
                AddWeeeSentOnAmount(databaseWrapper, aatf1SentOnReturn1);
                AddWeeeReceivedAmount(databaseWrapper, aatf1ReceivedReturn1);

                var aatf2ReceivedReturn1 = AddWeeeReceived(databaseWrapper, organisation, aatf2, @return);
                var aatf2ReusedReturn1 = AddWeeeReused(databaseWrapper, aatf2, @return);
                var aatf2SentOnReturn1 = AddWeeeSentOn(databaseWrapper, aatf2, @return);
                AddReturnAatf(databaseWrapper, aatf2, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, aatf2ReusedReturn1);
                AddReusedAmount(databaseWrapper, aatf2ReusedReturn1);
                AddWeeeSentOnAmount(databaseWrapper, aatf2SentOnReturn1);
                AddWeeeReceivedAmount(databaseWrapper, aatf2ReceivedReturn1);

                var aatf1ReceivedReturn2 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                var aatf1ReusedReturn2 = AddWeeeReused(databaseWrapper, aatf, @return2);
                var aatf1SentOnReturn2 = AddWeeeSentOn(databaseWrapper, aatf, @return2);
                AddReturnAatf(databaseWrapper, aatf, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, aatf1ReusedReturn2);
                AddReusedAmount(databaseWrapper, aatf1ReusedReturn2);
                AddWeeeSentOnAmount(databaseWrapper, aatf1SentOnReturn2);
                AddWeeeReceivedAmount(databaseWrapper, aatf1ReceivedReturn2);

                var aatf2ReceivedReturn2 = AddWeeeReceived(databaseWrapper, organisation, aatf2, @return2);
                var aatf2ReusedReturn2 = AddWeeeReused(databaseWrapper, aatf2, @return2);
                var aatf2SentOnReturn2 = AddWeeeSentOn(databaseWrapper, aatf2, @return2);
                AddReturnAatf(databaseWrapper, aatf2, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, aatf2ReusedReturn2);
                AddReusedAmount(databaseWrapper, aatf2ReusedReturn2);
                AddWeeeSentOnAmount(databaseWrapper, aatf2SentOnReturn2);
                AddWeeeReceivedAmount(databaseWrapper, aatf2ReceivedReturn2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, aatf1ReceivedReturn1, aatf1SentOnReturn1, aatf1ReusedReturn1, aatf);
                AssertAllTrue(databaseWrapper, @return2, aatf1ReceivedReturn2, aatf1SentOnReturn2, aatf1ReusedReturn2, aatf);
                AssertAllTrue(databaseWrapper, @return, aatf2ReceivedReturn1, aatf2SentOnReturn1, aatf2ReusedReturn1, aatf2);
                AssertAllTrue(databaseWrapper, @return2, aatf2ReceivedReturn2, aatf2SentOnReturn2, aatf2ReusedReturn2, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf2.Id).Should().BeTrue();

                AssertAllAatfDataTrue(databaseWrapper, @return, aatf2, aatf2ReceivedReturn1, aatf2ReusedReturn1, aatf2SentOnReturn1);
                AssertAllAatfDataTrue(databaseWrapper, @return2, aatf2, aatf2ReceivedReturn2, aatf2ReusedReturn2, aatf2SentOnReturn2);
                AssertAllAatfDataTrue(databaseWrapper, @return2, aatf, aatf1ReceivedReturn2, aatf1ReusedReturn2, aatf1SentOnReturn2);
                AssertAllAatfDataFalse(databaseWrapper, @return, aatf, aatf1ReceivedReturn1, aatf1ReusedReturn1, aatf1SentOnReturn1);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfInFirstQuarterAndMultipleAatfInSecondQuarter_FirstQuarterReturnShouldBeRemovedAndSecondQuarterReturnNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                var secondQuarterDate = new DateTime(2019, 7, 1);
                var firstQuarterDate = new DateTime(2019, 4, 1);
                // set approval date to 1st quarter
                aatf.UpdateDetails(aatf2.Name, aatf2.CompetentAuthority, aatf2.ApprovalNumber, aatf2.AatfStatus, aatf2.Organisation, aatf2.Size, firstQuarterDate, aatf2.LocalArea, aatf2.PanArea);
                // set approval date to 2nd quarter
                aatf2.UpdateDetails(aatf2.Name, aatf2.CompetentAuthority, aatf2.ApprovalNumber, aatf2.AatfStatus, aatf2.Organisation, aatf2.Size, secondQuarterDate, aatf2.LocalArea, aatf2.PanArea);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1, firstQuarterDate);
                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q2, secondQuarterDate);

                var aatf1ReceivedReturn1 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var aatf1ReusedReturn1 = AddWeeeReused(databaseWrapper, aatf, @return);
                var aatf1SentOnReturn1 = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, aatf1ReusedReturn1);
                AddReusedAmount(databaseWrapper, aatf1ReusedReturn1);
                AddWeeeSentOnAmount(databaseWrapper, aatf1SentOnReturn1);
                AddWeeeReceivedAmount(databaseWrapper, aatf1ReceivedReturn1);

                var aatf2ReceivedReturn2 = AddWeeeReceived(databaseWrapper, organisation, aatf2, @return2);
                var aatf2ReusedReturn2 = AddWeeeReused(databaseWrapper, aatf2, @return2);
                var aatf2SentOnReturn2 = AddWeeeSentOn(databaseWrapper, aatf2, @return2);
                AddReturnAatf(databaseWrapper, aatf2, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, aatf2ReusedReturn2);
                AddReusedAmount(databaseWrapper, aatf2ReusedReturn2);
                AddWeeeSentOnAmount(databaseWrapper, aatf2SentOnReturn2);
                AddWeeeReceivedAmount(databaseWrapper, aatf2ReceivedReturn2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, aatf1ReceivedReturn1, aatf1SentOnReturn1, aatf1ReusedReturn1, aatf);
                AssertAllTrue(databaseWrapper, @return2, aatf2ReceivedReturn2, aatf2SentOnReturn2, aatf2ReusedReturn2, aatf2);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1, 2 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf2.Id).Should().BeTrue();

                AssertAllTrue(databaseWrapper, @return2, aatf2ReceivedReturn2, aatf2SentOnReturn2, aatf2ReusedReturn2, aatf2);
                AssertAllAatfDataFalse(databaseWrapper, @return, aatf, aatf1ReceivedReturn1, aatf1ReusedReturn1, aatf1SentOnReturn1);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAeAndReturnsInDifferentQuarter_ReturnDataShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var ae = ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, organisation);
                var ae2 = ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, organisation);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                databaseWrapper.WeeeContext.Returns.Add(@return);
                databaseWrapper.WeeeContext.Returns.Add(@return2);
                databaseWrapper.WeeeContext.Aatfs.Add(ae);
                databaseWrapper.WeeeContext.Aatfs.Add(ae2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);
                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == return2.Id).Should().Be(1);

                await aatfDataAccess.RemoveAatfData(ae, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);
                databaseWrapper.WeeeContext.Returns.Count(r => r.Id == return2.Id).Should().Be(1);
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndSingleQuarterAndReturnInDifferentYear_AatfDataShouldBeRemovedAndOtherReturnDataNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper), quarterWindowFactory);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2020, QuarterType.Q1);

                SetupQuarterWindow(aatf.ComplianceYear, QuarterType.Q1);
                SetupQuarterWindow(2020, QuarterType.Q1);

                var aatf1ReceivedReturn1 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                var aatf1ReusedReturn1 = AddWeeeReused(databaseWrapper, aatf, @return);
                var aatf1SentOnReturn1 = AddWeeeSentOn(databaseWrapper, aatf, @return);
                AddReturnAatf(databaseWrapper, aatf, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, aatf1ReusedReturn1);
                AddReusedAmount(databaseWrapper, aatf1ReusedReturn1);
                AddWeeeSentOnAmount(databaseWrapper, aatf1SentOnReturn1);
                AddWeeeReceivedAmount(databaseWrapper, aatf1ReceivedReturn1);

                var aatf2ReceivedReturn1 = AddWeeeReceived(databaseWrapper, organisation, aatf2, @return);
                var aatf2ReusedReturn1 = AddWeeeReused(databaseWrapper, aatf2, @return);
                var aatf2SentOnReturn1 = AddWeeeSentOn(databaseWrapper, aatf2, @return);
                AddReturnAatf(databaseWrapper, aatf2, @return);
                AddReturnScheme(databaseWrapper, organisation, @return);
                AddReusedSite(databaseWrapper, aatf2ReusedReturn1);
                AddReusedAmount(databaseWrapper, aatf2ReusedReturn1);
                AddWeeeSentOnAmount(databaseWrapper, aatf2SentOnReturn1);
                AddWeeeReceivedAmount(databaseWrapper, aatf2ReceivedReturn1);

                var aatf1ReceivedReturn2 = AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                var aatf1ReusedReturn2 = AddWeeeReused(databaseWrapper, aatf, @return2);
                var aatf1SentOnReturn2 = AddWeeeSentOn(databaseWrapper, aatf, @return2);
                AddReturnAatf(databaseWrapper, aatf, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, aatf1ReusedReturn2);
                AddReusedAmount(databaseWrapper, aatf1ReusedReturn2);
                AddWeeeSentOnAmount(databaseWrapper, aatf1SentOnReturn2);
                AddWeeeReceivedAmount(databaseWrapper, aatf1ReceivedReturn2);

                var aatf2ReceivedReturn2 = AddWeeeReceived(databaseWrapper, organisation, aatf2, @return2);
                var aatf2ReusedReturn2 = AddWeeeReused(databaseWrapper, aatf2, @return2);
                var aatf2SentOnReturn2 = AddWeeeSentOn(databaseWrapper, aatf2, @return2);
                AddReturnAatf(databaseWrapper, aatf2, @return2);
                AddReturnScheme(databaseWrapper, organisation, @return2);
                AddReusedSite(databaseWrapper, aatf2ReusedReturn2);
                AddReusedAmount(databaseWrapper, aatf2ReusedReturn2);
                AddWeeeSentOnAmount(databaseWrapper, aatf2SentOnReturn2);
                AddWeeeReceivedAmount(databaseWrapper, aatf2ReceivedReturn2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                databaseWrapper.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return2.Id, 1));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                AssertAllTrue(databaseWrapper, @return, aatf1ReceivedReturn1, aatf1SentOnReturn1, aatf1ReusedReturn1, aatf);
                AssertAllTrue(databaseWrapper, @return2, aatf1ReceivedReturn2, aatf1SentOnReturn2, aatf1ReusedReturn2, aatf);
                AssertAllTrue(databaseWrapper, @return, aatf2ReceivedReturn1, aatf2SentOnReturn1, aatf2ReusedReturn1, aatf2);
                AssertAllTrue(databaseWrapper, @return2, aatf2ReceivedReturn2, aatf2SentOnReturn2, aatf2ReusedReturn2, aatf);

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 });

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf2.Id).Should().BeTrue();

                AssertAllAatfDataTrue(databaseWrapper, @return, aatf2, aatf2ReceivedReturn1, aatf2ReusedReturn1, aatf2SentOnReturn1);
                AssertAllAatfDataTrue(databaseWrapper, @return2, aatf2, aatf2ReceivedReturn2, aatf2ReusedReturn2, aatf2SentOnReturn2);
                AssertAllAatfDataTrue(databaseWrapper, @return2, aatf, aatf1ReceivedReturn2, aatf1ReusedReturn2, aatf1SentOnReturn2);
                AssertAllAatfDataFalse(databaseWrapper, @return, aatf, aatf1ReceivedReturn1, aatf1ReusedReturn1, aatf1SentOnReturn1);
            }
        }

        private void AssertAllAatfDataFalse(DatabaseWrapper databaseWrapper, Return @return, Aatf aatf, WeeeReceived received, WeeeReused reused,
            WeeeSentOn sentOn)
        {
            databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id && r.Aatf.Id == aatf.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReused.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReceivedAmount.Any(r => r.WeeeReceived.Id == received.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReusedSite.Any(r => r.WeeeReused.Id == reused.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReusedAmount.Any(r => r.WeeeReused.Id == reused.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeSentOnAmount.Any(r => r.WeeeSentOn.Id == sentOn.Id).Should().BeFalse();
        }

        private void AssertAllAatfDataTrue(DatabaseWrapper databaseWrapper, Return @return, Aatf aatf2, WeeeReceived received2, WeeeReused reused2,
            WeeeSentOn sentOn2)
        {
            databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id && r.Aatf.Id == aatf2.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReceivedAmount.Any(r => r.WeeeReceived.Id == received2.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReusedSite.Any(r => r.WeeeReused.Id == reused2.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReusedAmount.Any(r => r.WeeeReused.Id == reused2.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeSentOnAmount.Any(r => r.WeeeSentOn.Id == sentOn2.Id).Should().BeTrue();
        }

        private static void AddWeeeReceivedAmount(DatabaseWrapper databaseWrapper, WeeeReceived received)
        {
            databaseWrapper.WeeeContext.WeeeReceivedAmount.Add(new WeeeReceivedAmount(received, 1, 1, 1));
        }

        private static void AddWeeeSentOnAmount(DatabaseWrapper databaseWrapper, WeeeSentOn sentOn)
        {
            databaseWrapper.WeeeContext.WeeeSentOnAmount.Add(new WeeeSentOnAmount(sentOn, 1, 1, 1));
        }

        private static void AddReusedAmount(DatabaseWrapper databaseWrapper, WeeeReused reused)
        {
            databaseWrapper.WeeeContext.WeeeReusedAmount.Add(new WeeeReusedAmount(reused, 1, 1, 1));
        }

        private static void AddReusedSite(DatabaseWrapper databaseWrapper, WeeeReused reused)
        {
            databaseWrapper.WeeeContext.WeeeReusedSite.Add(new WeeeReusedSite(reused,
                ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper)));
        }

        private WeeeSentOn AddWeeeSentOn(DatabaseWrapper databaseWrapper, Aatf aatf, Return @return)
        {
            var weeeSentOn = new Domain.AatfReturn.WeeeSentOn(
                ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper),
                ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper), aatf, @return);

            databaseWrapper.WeeeContext.WeeeSentOn.Add(weeeSentOn);

            return weeeSentOn;
        }

        private WeeeReused AddWeeeReused(DatabaseWrapper databaseWrapper, Aatf aatf, Return @return)
        {
            var reused = new Domain.AatfReturn.WeeeReused(aatf, @return);
            databaseWrapper.WeeeContext.WeeeReused.Add(reused);
            return reused;
        }

        private WeeeReceived AddWeeeReceived(DatabaseWrapper databaseWrapper, Organisation organisation, Aatf aatf, Return @return)
        {
            return databaseWrapper.WeeeContext.WeeeReceived.Add(
                new Domain.AatfReturn.WeeeReceived(new Domain.Scheme.Scheme(organisation), aatf, @return));
        }

        private static void AddReturnScheme(DatabaseWrapper databaseWrapper, Organisation organisation, Return @return)
        {
            databaseWrapper.WeeeContext.ReturnScheme.Add(new Domain.AatfReturn.ReturnScheme(new Domain.Scheme.Scheme(organisation), @return));
        }

        private static void AddReturnAatf(DatabaseWrapper databaseWrapper, Aatf aatf, Return @return)
        {
            databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
        }

        private void AssertAllTrue(DatabaseWrapper databaseWrapper, Return @return, WeeeReceived received, WeeeSentOn sentOn, WeeeReused reused,
            Aatf aatf)
        {
            databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReceivedAmount.Any(r => r.WeeeReceived.Id == received.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeSentOnAmount.Any(r => r.WeeeSentOn.Id == sentOn.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReusedAmount.Any(r => r.WeeeReused.Id == reused.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReusedSite.Any(r => r.WeeeReused.Id == reused.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.Return.Id == @return.Id && r.AatfId == aatf.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.Return.Id == @return.Id && r.AatfId == aatf.Id).Should().BeTrue();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.Return.Id == @return.Id && r.AatfId == aatf.Id).Should().BeTrue();
        }

        private void AssertAllFalse(DatabaseWrapper databaseWrapper, Return @return, WeeeReceived received, WeeeSentOn sentOn, WeeeReused reused,
            Aatf aatf)
        {
            databaseWrapper.WeeeContext.ReturnReportOns.Any(r => r.Return.Id == @return.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.ReturnScheme.Any(r => r.Return.Id == @return.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReceivedAmount.Any(r => r.WeeeReceived.Id == received.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeSentOnAmount.Any(r => r.WeeeSentOn.Id == sentOn.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReusedAmount.Any(r => r.WeeeReused.Id == reused.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReusedSite.Any(r => r.WeeeReused.Id == reused.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.Return.Id == @return.Id && r.AatfId == aatf.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.Return.Id == @return.Id && r.AatfId == aatf.Id).Should().BeFalse();
            databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.Return.Id == @return.Id && r.AatfId == aatf.Id).Should().BeFalse();
        }

        private void SetupQuarterWindow(int year, QuarterType quarter)
        {
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(A<Quarter>.That.Matches(x =>
                    x.Year == year && x.Q == quarter)))
                .Returns(new QuarterWindow(DateTime.MaxValue, DateTime.MaxValue, quarter));
        }

        private void SetupQuarterWindow(int year, QuarterType quarter, DateTime startDate)
        {
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(A<Quarter>.That.Matches(x =>
                    x.Year == year && x.Q == quarter)))
                .Returns(new QuarterWindow(startDate, startDate, quarter));
        }
    }
}
