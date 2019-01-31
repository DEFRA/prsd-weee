namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.NonObligated;
    using RequestHandlers.DataReturns.FetchDataReturnForSubmission;
    using RequestHandlers.DataReturns.SubmitReturnVersion;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class AddNonObligatedRequestHandlerTests
    {
        private readonly RequestHandlers.AatfReturn.AddReturnRequestHandler requestHandler;
        private readonly IWeeeAuthorization authorization;
        private readonly IAddNonObligatedDataAccess dataAccess;
        
        public AddNonObligatedRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IAddNonObligatedDataAccess>();

            requestHandler = new RequestHandlers.AatfReturn.AddReturnRequestHandler(authorization, dataAccess);
        }
        /*
        [Fact]
        public async Task HandleAsync_WithValidRequest_NonObligatedWeeDomainIsMapped()
        {
            var operatorTest = new Operator(Guid.NewGuid(), message.OrganisationId);

            var aatfReturn = new Return(Guid.NewGuid(), 1, 1, 1, operatorTest);

            var nonObligatedWee = new List<NonObligatedWeee>();
            var categoryValues = new List<NonObligatedRequestValue>();
            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                categoryValues.Add(new NonObligatedRequestValue((int)category, (int)category, false));
            }

            var request = new AddNonObligatedRequest()
            {
                OrganisationId = Guid.NewGuid(),
                CategoryValues = categoryValues,
                Dcf = false
            };

            await requestHandler.HandleAsync(request);

            A.CallTo(() => nonObligatedWee.Add(new NonObligatedWeee(aatfReturn, categoryValue.CategoryId, message.Dcf, categoryValue.Tonnage)));
        }
        */
        //[Fact]
        //public Task HandleAsync_WithNoExternalAccess_ThrowsSecurityException()
        //{
        //    //IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
        //}
    }
}
