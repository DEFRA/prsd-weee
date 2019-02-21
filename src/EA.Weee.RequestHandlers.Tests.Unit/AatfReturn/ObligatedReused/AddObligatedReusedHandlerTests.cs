namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
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

    public class AddObligatedReusedHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddObligatedReusedDataAccess addObligatedReusedDataAccess;

        public AddObligatedReusedHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            addObligatedReusedDataAccess = A.Dummy<IAddObligatedReusedDataAccess>();
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
            var organisation = A.Fake<Organisation>();
            var @operator = new Operator(organisation);
            var aatfReturn = new Return(@operator, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);

            var weeeReused = new WeeeReused(
                await addObligatedReusedDataAccess.GetAatfId(organisation.Id),
                aatfReturn.Id);
            var weeeReusedAmount = new List<WeeeReusedAmount>();

            var categoryValues = new List<ObligatedValue>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                categoryValues.Add(new ObligatedValue((int)category, (int)category, (int)category));
            }

            var obligatedWeeeRequest = new AddObligatedReused
            {
                ReturnId = aatfReturn.Id,
                OrganisationId = organisation.Id,
                CategoryValues = categoryValues
            };
            
            foreach (var categoryValue in obligatedWeeeRequest.CategoryValues)
            {
                weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
            }
            
            var requestHandler = new AddObligatedReusedHandler(authorization, addObligatedReusedDataAccess);

            await requestHandler.HandleAsync(obligatedWeeeRequest);

            A.CallTo(() => addObligatedReusedDataAccess.Submit(A<List<WeeeReusedAmount>>.That.IsSameAs(weeeReusedAmount)));
        }
    }
}
