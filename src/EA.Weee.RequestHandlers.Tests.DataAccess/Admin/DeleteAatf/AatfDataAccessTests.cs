namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.DeleteAatf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.Admin;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Internal;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;

    public class AatfDataAccessTests
    {
        private readonly Fixture fixture;

        public AatfDataAccessTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async void RemoveAatf_GivenAatf_AatfShouldShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

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
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

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
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, null);

                AddWeeSentOn(databaseWrapper, aatf, @return);

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
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, null);

                AddWeeReused(databaseWrapper, aatf, @return);

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
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

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
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarter_AatfDataAndStartedReturnShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var flags = new CanApprovalDateBeChangedFlags();

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() {1}, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeFalse();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterWithReturnInSameQuarterButDifferentYear_OtherYearReturnsShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2020, QuarterType.Q1);

                var flags = new CanApprovalDateBeChangedFlags();

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                databaseWrapper.WeeeContext.Returns.Add(@return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndSingleQuarterWithReturnInSameYearButDifferentQuarter_OtherYearReturnsShouldNotBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                var flags = new CanApprovalDateBeChangedFlags();

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                databaseWrapper.WeeeContext.Returns.Add(@return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenSingleAatfAndMultipleQuarter_AatfDataAndStartedReturnShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                var flags = new CanApprovalDateBeChangedFlags();

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                AddWeeReused(databaseWrapper, aatf, @return2);
                AddWeeSentOn(databaseWrapper, aatf, @return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1, 2 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeFalse();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndSingleQuarter_AatfDataShouldBeRemovedAndReturnNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var flags = new CanApprovalDateBeChangedFlags();
                flags |= CanApprovalDateBeChangedFlags.HasMultipleFacility;

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                AddWeeeReceived(databaseWrapper, organisation, aatf2, @return);
                AddWeeReused(databaseWrapper, aatf2, @return);
                AddWeeSentOn(databaseWrapper, aatf2, @return);

                databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                databaseWrapper.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf2, @return));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id && r.Aatf.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id && r.Aatf.Id == aatf2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id && r.Aatf.Id == aatf.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.ReturnAatfs.Any(r => r.Return.Id == @return.Id && r.Aatf.Id == aatf2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndSingleQuarterAndReturnInDifferentQuarter_AatfDataShouldBeRemovedAndOtherReturnDataNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                var flags = new CanApprovalDateBeChangedFlags();
                flags |= CanApprovalDateBeChangedFlags.HasMultipleFacility;

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                AddWeeReused(databaseWrapper, aatf, @return2);
                AddWeeSentOn(databaseWrapper, aatf, @return2);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndSingleQuarterAndReturnInDifferentYear_AatfDataShouldBeRemovedAndOtherReturnDataNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2020, QuarterType.Q1);

                var flags = new CanApprovalDateBeChangedFlags();
                flags |= CanApprovalDateBeChangedFlags.HasMultipleFacility;

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                AddWeeReused(databaseWrapper, aatf, @return2);
                AddWeeSentOn(databaseWrapper, aatf, @return2);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
            }
        }

        [Fact]
        public async void RemoveAatfData_GivenMultipleAatfAndMultipleQuarter_AatfDataShouldBeRemovedAndReturnNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var aatfDataAccess = new AatfDataAccess(databaseWrapper.WeeeContext, GetGenericDataAccess(databaseWrapper));

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q2);

                var flags = new CanApprovalDateBeChangedFlags();
                flags |= CanApprovalDateBeChangedFlags.HasMultipleFacility;

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return);
                AddWeeReused(databaseWrapper, aatf, @return);
                AddWeeSentOn(databaseWrapper, aatf, @return);

                AddWeeeReceived(databaseWrapper, organisation, aatf2, @return);
                AddWeeReused(databaseWrapper, aatf2, @return);
                AddWeeSentOn(databaseWrapper, aatf2, @return);

                AddWeeeReceived(databaseWrapper, organisation, aatf, @return2);
                AddWeeReused(databaseWrapper, aatf, @return2);
                AddWeeSentOn(databaseWrapper, aatf, @return2);

                AddWeeeReceived(databaseWrapper, organisation, aatf2, @return2);
                AddWeeReused(databaseWrapper, aatf2, @return2);
                AddWeeSentOn(databaseWrapper, aatf2, @return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return2.Id).Should().BeTrue();

                await aatfDataAccess.RemoveAatfData(aatf, new List<int>() { 1, 2 }, flags);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Returns.Any(r => r.Id == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.Aatfs.Any(r => r.Id == aatf2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf.Id && r.ReturnId == @return2.Id).Should().BeFalse();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeSentOn.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return2.Id).Should().BeTrue();
                databaseWrapper.WeeeContext.WeeeReceived.Any(r => r.AatfId == aatf2.Id && r.ReturnId == @return2.Id).Should().BeTrue();
            }
        }

        private void AddWeeSentOn(DatabaseWrapper databaseWrapper, Aatf aatf, Return @return)
        {
            databaseWrapper.WeeeContext.WeeeSentOn.Add(new Domain.AatfReturn.WeeeSentOn(
                ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper),
                ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper), aatf, @return));
        }

        private void AddWeeReused(DatabaseWrapper databaseWrapper, Aatf aatf, Return @return)
        {
            databaseWrapper.WeeeContext.WeeeReused.Add(new Domain.AatfReturn.WeeeReused(aatf, @return));
        }

        private WeeeReceived AddWeeeReceived(DatabaseWrapper databaseWrapper, Organisation organisation, Aatf aatf, Return @return)
        {
            return databaseWrapper.WeeeContext.WeeeReceived.Add(
                new Domain.AatfReturn.WeeeReceived(new Domain.Scheme.Scheme(organisation), aatf, @return));
        }
    }
}
