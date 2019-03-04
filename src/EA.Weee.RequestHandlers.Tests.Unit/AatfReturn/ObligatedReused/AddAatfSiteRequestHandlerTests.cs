namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddAatfSiteRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorisation;
        private readonly IAddAatfSiteDataAccess addAatfSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedReusedDataAccess obligatedReusedDataAccess; 
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess; 
        private readonly WeeeContext weeeContext;
        private readonly AddAatfSiteRequestHandler handler;

        public AddAatfSiteRequestHandlerTests()
        {
            authorisation = A.Fake<IWeeeAuthorization>();
            addAatfSiteDataAccess = A.Fake<IAddAatfSiteDataAccess>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            obligatedReusedDataAccess = A.Fake<IObligatedReusedDataAccess>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            weeeContext = A.Fake<WeeeContext>();

            handler = new AddAatfSiteRequestHandler(weeeContext, authorisation, addAatfSiteDataAccess, genericDataAccess, organisationDetailsDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddAatfSiteRequestHandler(weeeContext, authorization, addAatfSiteDataAccess, genericDataAccess, organisationDetailsDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithValidInput_SubmitIsCalledCorrectly()
        {
            var aatf = A.Fake<Aatf>();
            var organisation = A.Fake<Organisation>();
            var @operator = new Operator(organisation);
            var aatfReturn = new Return(@operator, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);
            var aatfAddress = new AatfAddress(
                "Name",
                "Address",
                A.Dummy<string>(),
                "TownOrCity",
                A.Dummy<string>(),
                A.Dummy<string>(),
                A.Dummy<Country>());

            var weeeReused = new WeeeReused(aatf.Id, aatfReturn.Id);

            A.CallTo(() => genericDataAccess.GetManyByExpression(A<WeeeReusedByAatfIdAndReturnIdSpecification>._)).Returns(new List<WeeeReused>() { weeeReused });

            var weeeReusedSite = new WeeeReusedSite(weeeReused, aatfAddress);

            var addressData = A.Fake<AddressData>();

            addressData.Name = "Name";
            addressData.Address1 = "Address1";
            addressData.TownOrCity = "TownOrCity";
            addressData.CountryId = Guid.NewGuid();

            var addAatfSiteRequest = new AddAatfSite
            {
                OrganisationId = organisation.Id,
                ReturnId = aatfReturn.Id,
                AatfId = aatf.Id,
                AddressData = addressData
            };

            await handler.HandleAsync(addAatfSiteRequest);

            A.CallTo(() => addAatfSiteDataAccess.Submit(A<WeeeReusedSite>.That.IsSameAs(weeeReusedSite)));
        }
    }
}
