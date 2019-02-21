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
    using EA.Weee.RequestHandlers.AatfReturn.Obligated;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.ObligatedReceived;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddObligatedReceivedHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddObligatedReceivedDataAccess addObligatedReceivedDataAccess;
        public AddObligatedReceivedHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            addObligatedReceivedDataAccess = A.Dummy<IAddObligatedReceivedDataAccess>();
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
            var @operator = new Operator(organisation);
            var aatfReturn = new Return(@operator, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);

            var weeeReceived = new WeeeReceived(
                await addObligatedReceivedDataAccess.GetSchemeId(organisation.Id),
                await addObligatedReceivedDataAccess.GetAatfId(organisation.Id),
                aatfReturn.Id);
            var weeeReceivedAmount = new List<WeeeReceivedAmount>();

            var categoryValues = new List<ObligatedReceivedValue>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                categoryValues.Add(new ObligatedReceivedValue((int)category, (int)category, (int)category));
            }

            var obligatedWeeeRequest = new AddObligatedReceived
            {
                ReturnId = aatfReturn.Id,
                OrganisationId = organisation.Id,
                CategoryValues = categoryValues
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
