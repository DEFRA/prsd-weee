﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedReceived;
    using RequestHandlers.Security;
    using Requests.AatfReturn.Obligated;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;

    public class AddObligatedReceivedHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedReceivedDataAccess addObligatedReceivedDataAccess;

        public AddObligatedReceivedHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            addObligatedReceivedDataAccess = A.Dummy<IObligatedReceivedDataAccess>();
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new AddObligatedReceivedHandler(authorization, addObligatedReceivedDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddObligatedReceived>());

            await action.Should().ThrowAsync<SecurityException>();
        }
        
        [Fact]
        public async Task HandleAsync_WithValidInput_SubmittedIsCalledCorrectly()
        {
            var organisation = A.Fake<Organisation>();
            var aatf = A.Fake<Aatf>();
            var scheme = A.Fake<Scheme>();
            var @operator = new Operator(organisation);
            var aatfReturn = new Return(@operator, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);

            var weeeReceived = new WeeeReceived(
                scheme.Id,
                aatf.Id,
                aatfReturn.Id);
            var weeeReceivedAmount = new List<WeeeReceivedAmount>();

            var categoryValues = new List<ObligatedValue>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                categoryValues.Add(new ObligatedValue(Guid.NewGuid(), (int)category, (int)category, (int)category));
            }

            var obligatedWeeeRequest = new AddObligatedReceived
            {
                ReturnId = aatfReturn.Id,
                OrganisationId = organisation.Id,
                CategoryValues = categoryValues,
                SchemeId = scheme.Id,
                AatfId = aatf.Id
            };
            
            foreach (var categoryValue in obligatedWeeeRequest.CategoryValues)
            {
                weeeReceivedAmount.Add(new WeeeReceivedAmount(weeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
            }
            
            var requestHandler = new AddObligatedReceivedHandler(authorization, addObligatedReceivedDataAccess);

            await requestHandler.HandleAsync(obligatedWeeeRequest);

            A.CallTo(() => addObligatedReceivedDataAccess.Submit(A<List<WeeeReceivedAmount>>.That.IsSameAs(weeeReceivedAmount)));
        }
    }
}
