﻿namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;

    public class GetReportOnQuestionHandler : IRequestHandler<GetReportOnQuestion, List<ReportOnQuestion>>
    {
        private readonly IGenericDataAccess dataAccess;

        public GetReportOnQuestionHandler(IGenericDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<List<ReportOnQuestion>> HandleAsync(GetReportOnQuestion message)
        {
            var questions = await dataAccess.GetAll<Domain.AatfReturn.ReportOnQuestion>();

            return questions.Select(s => new ReportOnQuestion(s.Id, s.Question, s.Description, s.ParentId ?? default(int))).ToList();
        }
    }
}
