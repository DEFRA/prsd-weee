namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using ReportOnQuestion = Core.AatfReturn.ReportOnQuestion;

    public class AddReturnReportOnHandler : IRequestHandler<AddReturnReportOn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;

        public AddReturnReportOnHandler(IWeeeAuthorization authorization,
            IGenericDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(AddReturnReportOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var returnReportOn = new List<ReturnReportOn>();

            foreach (var question in message.SelectedOptions)
            {
                //if (question.ParentId != 0)
                //{
                //    if (message.SelectedOptions.Where(q => q.Id == question.ParentId).FirstOrDefault().SelectedValue == ReportOnQuestion.SelectedValue.Yes)
                //    {

                //    }
                //}
                returnReportOn.Add(new ReturnReportOn(message.ReturnId, question.Id));
            }

            await dataAccess.AddMany<ReturnReportOn>(returnReportOn);

            return true;
        }
    }
}
