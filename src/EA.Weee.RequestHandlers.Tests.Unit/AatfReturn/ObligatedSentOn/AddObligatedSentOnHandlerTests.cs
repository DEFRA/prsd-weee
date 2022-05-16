namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Requests.Aatf;
    using Xunit;

    public class AddObligatedSentOnHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedSentOnDataAccess addObligatedSentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public AddObligatedSentOnHandlerTests()
        {
            genericDataAccess = A.Fake<IGenericDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            addObligatedSentOnDataAccess = A.Dummy<IObligatedSentOnDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddObligatedSentOnHandler(authorization, addObligatedSentOnDataAccess, genericDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddObligatedSentOn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithValidInput_SubmittedIsCalledCorrectly()
        {
            var aatf = A.Fake<Aatf>();
            var weeeSentOnId = Guid.NewGuid();
            var siteAddress = A.Fake<AatfAddress>();
            var aatfReturn = ReturnHelper.GetReturn();

            var weeeSentOn = new WeeeSentOn(
                aatf.Id,
                aatfReturn.Id,
                siteAddress.Id);

            var weeeSentOnAmount = new List<WeeeSentOnAmount>();

            var categoryValues = new List<TonnageValues>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                categoryValues.Add(new TonnageValues(Guid.NewGuid(), (int)category, (int)category, (int)category));
            }

            var obligatedWeeeRequest = new AddObligatedSentOn
            {
                AatfId = aatf.Id,
                ReturnId = aatfReturn.Id,
                OrganisationId = aatfReturn.Organisation.Id,
                CategoryValues = categoryValues,
                SiteAddressId = siteAddress.Id,
                WeeeSentOnId = weeeSentOnId
            };

            foreach (var categoryValue in obligatedWeeeRequest.CategoryValues)
            {
                weeeSentOnAmount.Add(new WeeeSentOnAmount(weeeSentOn, categoryValue.CategoryId, categoryValue.FirstTonnage, categoryValue.SecondTonnage));
            }

            var requestHandler = new AddObligatedSentOnHandler(authorization, addObligatedSentOnDataAccess, genericDataAccess);

            await requestHandler.HandleAsync(obligatedWeeeRequest);

            A.CallTo(() => addObligatedSentOnDataAccess.Submit(A<List<WeeeSentOnAmount>>.That.IsSameAs(weeeSentOnAmount)));
        }
    }
}
