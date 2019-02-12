namespace EA.Weee.DataAccess.Tests.Integration
{
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;

    public class AddReturnSchemeIntegration
    {
        [Fact]
        public async Task CanCreateReturnScheme()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var name = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string crn = "ABC12345";

                var organisation = Organisation.CreateRegisteredCompany(name, crn, tradingName);

                context.Organisations.Add(organisation);
                await context.SaveChangesAsync();

                var scheme = new Scheme(organisation.Id);

                context.Schemes.Add(scheme);

                var operatorTest = new Operator(organisation);
                var quarter = new Quarter(2019, QuarterType.Q1);
                var @return = new Return(operatorTest, quarter, ReturnStatus.Created);

                context.Returns.Add(@return);
                await context.SaveChangesAsync();

                var dataAccess = new ReturnSchemeDataAccess(database.WeeeContext);

                var returnScheme = new ReturnScheme(scheme, @return);

                await dataAccess.Submit(returnScheme);

                var returnSchemeReturned = context.ReturnScheme.Where(o => o.Id == returnScheme.Id).First();
                returnSchemeReturned.Should().NotBeNull();
                returnSchemeReturned.ReturnId.Should().Be(@return.Id);
                returnSchemeReturned.SchemeId.Should().Be(scheme.Id);
            }
        }
    }
}
