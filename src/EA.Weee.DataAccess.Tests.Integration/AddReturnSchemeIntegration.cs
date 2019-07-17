namespace EA.Weee.DataAccess.Tests.Integration
{
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Organisation = Domain.Organisation;
    using Return = Domain.AatfReturn.Return;
    using ReturnScheme = Domain.AatfReturn.ReturnScheme;
    using Scheme = Domain.Scheme.Scheme;

    public class AddReturnSchemeIntegration
    {
        [Fact]
        public async Task CanCreateReturnScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var name = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string crn = "ABC12345";

                var organisation = Organisation.Organisation.CreateRegisteredCompany(name, crn, tradingName);
                
                context.Organisations.Add(organisation);
                await context.SaveChangesAsync();

                var scheme = new Scheme(organisation.Id);
                context.Schemes.Add(scheme);

                var quarter = new Quarter(2019, QuarterType.Q1);
                var @return = new Return(organisation, quarter, database.Model.AspNetUsers.First().Id, FacilityType.Aatf);

                context.Returns.Add(@return);
                await context.SaveChangesAsync();

                var dataAccess = new ReturnSchemeDataAccess(database.WeeeContext);

                var returnScheme = new ReturnScheme(scheme, @return);

                await dataAccess.Submit(new List<ReturnScheme> { returnScheme });

                var returnSchemeReturned = context.ReturnScheme.First(o => o.Id == returnScheme.Id);
                returnSchemeReturned.Should().NotBeNull();
                returnSchemeReturned.ReturnId.Should().Be(@return.Id);
                returnSchemeReturned.SchemeId.Should().Be(scheme.Id);
            }
        }
    }
}
