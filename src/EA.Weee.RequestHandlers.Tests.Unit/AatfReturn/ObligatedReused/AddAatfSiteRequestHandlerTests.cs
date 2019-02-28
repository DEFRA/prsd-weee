namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddAatfSiteRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorisation;
        private readonly AddAatfSiteDataAccess addAatfSiteDataAccess;
        private readonly ObligatedReusedDataAccess obligatedReusedDataAccess; 
        private readonly WeeeContext weeeContext;

        public AddAatfSiteRequestHandlerTests()
        {
            authorisation = A.Fake<IWeeeAuthorization>();
            addAatfSiteDataAccess = A.Dummy<AddAatfSiteDataAccess>();
            obligatedReusedDataAccess = A.Dummy<ObligatedReusedDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddAatfSiteRequestHandler(weeeContext, authorization, addAatfSiteDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        //public async Task HandleAsync_WithValidInput_SubmitIsCalledCorrectly()
        //{
        //    var aatf = A.Fake<Aatf>();
        //    var organisation = A.Fake<Organisation>();
        //    var @operator = new Operator(organisation);
        //    var aatfReturn = new Return(@operator, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);
        //    var aatfAddress = new AatfAddress(
        //        "Name",
        //        "Address",
        //        A.Dummy<string>(),
        //        "TownOrCity",
        //        A.Dummy<string>(),
        //        A.Dummy<string>(),
        //        A.Dummy<Guid>());

        //    var weeeReused = new WeeeReused(aatf.Id, aatfReturn.Id);

        //    var reusedHandler = new AddObligatedReusedHandler(authorisation, obligatedReusedDataAccess);

        //    var reusedRequest = new AddObligatedReused()
        //    {
        //        AatfId = aatf.Id,
        //        OrganisationId = organisation.Id,
        //        ReturnId = aatfReturn.Id,
        //        CategoryValues = A.Fake<IList<ObligatedValue>>()
        //    };

        //    await reusedHandler.HandleAsync(reusedRequest);

        //    var weeeReusedSite = new WeeeReusedSite(weeeReused, aatfAddress);

        //    var addressData = A.Fake<AddressData>();

        //    addressData.Name = "Name";
        //    addressData.Address1 = "Address1";
        //    addressData.TownOrCity = "TownOrCity";
        //    addressData.CountryId = Guid.NewGuid();

        //    var addAatfSiteRequest = new AddAatfSite
        //    {
        //        OrganisationId = organisation.Id,
        //        ReturnId = aatfReturn.Id,
        //        AatfId = aatf.Id,
        //        AddressData = addressData
        //    };

        //    var handler = new AddAatfSiteRequestHandler(weeeContext, authorisation, addAatfSiteDataAccess);

        //    await handler.HandleAsync(addAatfSiteRequest);

        //    A.CallTo(() => addAatfSiteDataAccess.Submit(A<WeeeReusedSite>.That.IsSameAs(weeeReusedSite)));
        //}
    }
}
