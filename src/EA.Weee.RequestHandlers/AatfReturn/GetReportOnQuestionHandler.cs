namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class GetReportOnQuestionHandler : IRequestHandler<GetReportOnQuestion, List<ReportOnQuestion>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;

        public GetReportOnQuestionHandler(IWeeeAuthorization authorization, IGenericDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<ReportOnQuestion>> HandleAsync(GetReportOnQuestion message)
        {
            authorization.EnsureCanAccessExternalArea();

            var questions = await dataAccess.GetAll<Domain.AatfReturn.ReportOnQuestion>();

            return questions.Select(s => new ReportOnQuestion(s.Id, s.Question, s.Description, s.ParentId ?? default(int), s.AlternativeDescription, s.Title)).ToList();
        }
    }
}
