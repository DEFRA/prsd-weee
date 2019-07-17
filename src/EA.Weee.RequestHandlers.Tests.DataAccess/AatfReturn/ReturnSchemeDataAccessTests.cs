namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class ReturnSchemeDataAccessTests
    {
        [Fact]
        public async Task RemoveReturnScheme_GivenSchemeIdsAndReturn_EntitiesShouldBeRemoved()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new ReturnSchemeDataAccess(database.WeeeContext);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, @return.Organisation);
                var scheme = new EA.Weee.Domain.Scheme.Scheme(@return.Organisation);
                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return);
                var weeeReceivedAmounts = new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, null, null);
                var returnScheme = new Domain.AatfReturn.ReturnScheme(scheme, @return);

                database.WeeeContext.Returns.Add(@return);
                database.WeeeContext.Organisations.Add(@return.Organisation);
                database.WeeeContext.Aatfs.Add(aatf);
                database.WeeeContext.Schemes.Add(scheme);
                database.WeeeContext.WeeeReceived.Add(weeeReceived);
                database.WeeeContext.WeeeReceivedAmount.Add(weeeReceivedAmounts);
                database.WeeeContext.ReturnScheme.Add(returnScheme);

                await database.WeeeContext.SaveChangesAsync();

                await dataAccess.RemoveReturnScheme(new List<Guid>() {scheme.Id}, @return.Id);

                database.WeeeContext.ReturnScheme.Count(r => r.SchemeId == scheme.Id).Should().Be(0);
                database.WeeeContext.WeeeReceived.Count(r => r.SchemeId == scheme.Id).Should().Be(0);
                database.WeeeContext.WeeeReceivedAmount.Count(r => r.WeeeReceived.Id == weeeReceived.Id).Should().Be(0);

                database.WeeeContext.Schemes.Count(s => s.Id == scheme.Id).Should().Be(1);
                database.WeeeContext.Returns.Count(r => r.Id == @return.Id).Should().Be(1);
            }
        }

        [Fact]
        public async Task RemoveReturnScheme_GivenSchemeIdsAndReturn_DataNotRelatedToSchemeShouldNotBeRemoved()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new ReturnSchemeDataAccess(database.WeeeContext);

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, @return.Organisation);

                var scheme = new EA.Weee.Domain.Scheme.Scheme(@return.Organisation);
                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return);
                var weeeReceivedAmounts = new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, null, null);
                var returnScheme = new Domain.AatfReturn.ReturnScheme(scheme, @return);

                var scheme2 = new EA.Weee.Domain.Scheme.Scheme(@return.Organisation);
                var weeeReceived2 = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme2, aatf, @return);
                var weeeReceivedAmounts2 = new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived2, 1, null, null);
                var returnScheme2 = new Domain.AatfReturn.ReturnScheme(scheme2, @return);

                database.WeeeContext.Returns.Add(@return);
                database.WeeeContext.Organisations.Add(@return.Organisation);
                database.WeeeContext.Aatfs.Add(aatf);
                database.WeeeContext.Schemes.Add(scheme);
                database.WeeeContext.Schemes.Add(scheme2);
                database.WeeeContext.WeeeReceived.Add(weeeReceived);
                database.WeeeContext.WeeeReceived.Add(weeeReceived2);
                database.WeeeContext.WeeeReceivedAmount.Add(weeeReceivedAmounts);
                database.WeeeContext.WeeeReceivedAmount.Add(weeeReceivedAmounts2);
                database.WeeeContext.ReturnScheme.Add(returnScheme);
                database.WeeeContext.ReturnScheme.Add(returnScheme2);

                await database.WeeeContext.SaveChangesAsync();

                await dataAccess.RemoveReturnScheme(new List<Guid>() { scheme.Id }, @return.Id);

                database.WeeeContext.ReturnScheme.Count(r => r.SchemeId == scheme.Id).Should().Be(0);
                database.WeeeContext.ReturnScheme.Count(r => r.SchemeId == scheme2.Id).Should().Be(1);
                database.WeeeContext.WeeeReceived.Count(r => r.SchemeId == scheme.Id).Should().Be(0);
                database.WeeeContext.WeeeReceived.Count(r => r.SchemeId == scheme2.Id).Should().Be(1);
                database.WeeeContext.WeeeReceivedAmount.Count(r => r.WeeeReceived.Id == weeeReceived.Id).Should().Be(0);
                database.WeeeContext.WeeeReceivedAmount.Count(r => r.WeeeReceived.Id == weeeReceived2.Id).Should().Be(1);
            }
        }
    }
}
