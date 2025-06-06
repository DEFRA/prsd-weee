﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReceived
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Requests.Aatf;
    using Xunit;

    public class AddObligatedReusedHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedReusedDataAccess addObligatedReusedDataAccess;

        public AddObligatedReusedHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            addObligatedReusedDataAccess = A.Dummy<IObligatedReusedDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddObligatedReusedHandler(authorization, addObligatedReusedDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddObligatedReused>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WithValidInput_SubmittedIsCalledCorrectly()
        {
            var aatf = A.Fake<Aatf>();
            var aatfReturn = ReturnHelper.GetReturn();

            var weeeReused = new WeeeReused(
                aatf.Id,
                aatfReturn.Id);
            var weeeReusedAmount = new List<WeeeReusedAmount>();

            var categoryValues = new List<TonnageValues>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                categoryValues.Add(new TonnageValues(Guid.NewGuid(), (int)category, (int)category, (int)category));
            }

            var obligatedWeeeRequest = new AddObligatedReused
            {
                AatfId = aatf.Id,
                ReturnId = aatfReturn.Id,
                OrganisationId = aatfReturn.Organisation.Id,
                CategoryValues = categoryValues
            };

            foreach (var categoryValue in obligatedWeeeRequest.CategoryValues)
            {
                weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, categoryValue.CategoryId, categoryValue.FirstTonnage, categoryValue.SecondTonnage));
            }

            var requestHandler = new AddObligatedReusedHandler(authorization, addObligatedReusedDataAccess);

            await requestHandler.HandleAsync(obligatedWeeeRequest);

            A.CallTo(() => addObligatedReusedDataAccess.Submit(A<List<WeeeReusedAmount>>.That.IsSameAs(weeeReusedAmount)));
        }
    }
}
