namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.DeleteAatf
{
    using System;
    using System.Linq;
    using AutoFixture;
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

                databaseWrapper.WeeeContext.WeeeSentOn.Add(new Domain.AatfReturn.WeeeSentOn(
                    ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper),
                    ObligatedWeeeIntegrationCommon.CreateAatfAddress(databaseWrapper), aatf, @return));
                
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

                databaseWrapper.WeeeContext.WeeeReused.Add(new Domain.AatfReturn.WeeeReused(aatf, @return));

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

                databaseWrapper.WeeeContext.WeeeReceived.Add(
                    new Domain.AatfReturn.WeeeReceived(new Domain.Scheme.Scheme(organisation), aatf, @return));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await Xunit.Record.ExceptionAsync(() => aatfDataAccess.RemoveAatf(aatf.Id));

                result.Should().BeOfType<InvalidOperationException>();
            }
        }
    }
}
