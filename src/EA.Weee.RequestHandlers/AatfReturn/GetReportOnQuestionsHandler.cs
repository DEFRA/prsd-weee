namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public class GetReportOnQuestions
    {
        private readonly IGenericDataAccess dataAccess;

        public GetReportOnQuestions(IGenericDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<List<ReportOnQuestion>> GetQuestions()
        {
            return await dataAccess.GetAll<ReportOnQuestion>();
        }
    }
}
