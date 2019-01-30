namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn.NonObligated;
    using RequestHandlers.DataReturns.FetchDataReturnForSubmission;
    using RequestHandlers.DataReturns.SubmitReturnVersion;
    using RequestHandlers.Security;
    using Requests.DataReturns;
    using Weee.Tests.Core;
    using Xunit;

    public class AddNonObligatedRequestHandlerTests
    {
        private readonly AddNonObligatedRequestHandler requestHandler;
        private readonly IWeeeAuthorization authorization;
        private readonly IAddNonObligatedDataAccess dataAccess;
        
        public AddNonObligatedRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IAddNonObligatedDataAccess>();

            requestHandler = new AddNonObligatedRequestHandler(authorization, dataAccess);
        }

        //[Fact]
        //public Task HandleAsync_WithNoExternalAccess_ThrowsSecurityException()
        //{
        //    //IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
        //}
    }
}
