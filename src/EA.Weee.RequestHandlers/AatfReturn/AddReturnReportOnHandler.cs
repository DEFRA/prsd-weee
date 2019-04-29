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
        private const string DcfYes = "Yes";

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

            foreach (var option in message.SelectedOptions)
            {
                returnReportOn.Add(new ReturnReportOn(message.ReturnId, option));
            }

            if (message.DcfSelectedValue == DcfYes)
            {
                var dcfQuestion = message.Options.Where(q => q.ParentId != default(int)).FirstOrDefault();
                bool isParentSelected = message.SelectedOptions.Contains(dcfQuestion.ParentId ?? default(int));

                if (isParentSelected)
                {
                    returnReportOn.Add(new ReturnReportOn(message.ReturnId, dcfQuestion.Id));
                }
            }

            await dataAccess.AddMany<ReturnReportOn>(returnReportOn);

            return true;
        }
    }
}