namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using FakeItEasy;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.NonObligated;
    using RequestHandlers.Security;

    public class AddNonObligatedRequestHandlerTests
    {
        //private readonly AddNonObligatedRequestHandler requestHandler;
        //private readonly IWeeeAuthorization authorization;
        //private readonly IAddNonObligatedDataAccess dataAccess;
        //private readonly IReturnDataAccess returnDataAccess;
        //public AddNonObligatedRequestHandlerTests()
        //{
        //    authorization = A.Fake<IWeeeAuthorization>();
        //    dataAccess = A.Fake<IAddNonObligatedDataAccess>();

        //    requestHandler = new AddNonObligatedRequestHandler(authorization, dataAccess, returnDataAccess);
        //}
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
