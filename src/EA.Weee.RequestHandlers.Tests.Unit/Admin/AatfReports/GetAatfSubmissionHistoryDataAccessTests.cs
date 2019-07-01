namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin.AatfReports;

    public class GetAatfSubmissionHistoryDataAccessTests
    {
        private readonly GetAatfSubmissionHistoryDataAccess dataAccess;
        private readonly WeeeContext context;

        public GetAatfSubmissionHistoryDataAccessTests()
        {
            context = A.Fake<WeeeContext>();

            dataAccess = new GetAatfSubmissionHistoryDataAccess(context);
        }
    }
}
