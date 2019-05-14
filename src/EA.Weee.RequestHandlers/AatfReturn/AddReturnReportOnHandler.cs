namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using ReturnReportOn = EA.Weee.Domain.AatfReturn.ReturnReportOn;

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

            if (message.DeselectedOptions != null && message.DeselectedOptions.Count != 0)
            {
                foreach (var deselected in message.DeselectedOptions)
                {
                    switch (deselected)
                    {
                        case (int)ReportOnQuestionEnum.WeeeReceived:
                            DeleteWeeeReceivedData(message.ReturnId);
                            break;
                        case (int)ReportOnQuestionEnum.WeeeSentOn:
                            DeleteWeeeSentOnData(message.ReturnId);
                            break;
                        case (int)ReportOnQuestionEnum.WeeeReused:
                            DeleteWeeeReusedData(message.ReturnId);
                            break;
                        case (int)ReportOnQuestionEnum.NonObligated:
                            DeleteNonObligatedData(message.ReturnId);
                            break;
                        case (int)ReportOnQuestionEnum.NonObligatedDcf:
                            DeleteNonObligatedData(message.ReturnId);
                            break;
                        default:
                            break;
                    }
                }
            }

            var returnReportOn = new List<ReturnReportOn>();

            foreach (var option in message.SelectedOptions)
            {
                returnReportOn.Add(new ReturnReportOn(message.ReturnId, option));
            }

            if (message.DcfSelectedValue == DcfYes)
            {
                bool isParentSelected = message.SelectedOptions.Contains((int?)ReportOnQuestionEnum.NonObligated ?? default(int));

                if (isParentSelected)
                {
                    returnReportOn.Add(new ReturnReportOn(message.ReturnId, (int)ReportOnQuestionEnum.NonObligatedDcf));
                }
            }
            var oldReturnOptions = await dataAccess.GetManyByReturnId<ReturnReportOn>(message.ReturnId);
            dataAccess.RemoveMany<ReturnReportOn>(oldReturnOptions);
            await dataAccess.AddMany<ReturnReportOn>(returnReportOn);

            return true;
        }

        private async void DeleteWeeeReceivedData(Guid returnId)
        {
            var weeeReceiveds = await dataAccess.GetManyByReturnId<WeeeReceived>(returnId);
            var weeeReceivedAmounts = new List<WeeeReceivedAmount>();

            foreach (var weeeReceived in weeeReceiveds)
            {
                weeeReceivedAmounts.AddRange((await dataAccess.GetAll<WeeeReceivedAmount>()).Where(w => w.WeeeReceived.Id == weeeReceived.Id));
            }

            dataAccess.RemoveMany<WeeeReceivedAmount>(weeeReceivedAmounts);
            dataAccess.RemoveMany<WeeeReceived>(weeeReceiveds);
        }

        private async void DeleteWeeeReusedData(Guid returnId)
        {
            var weeeReuseds = await dataAccess.GetManyByReturnId<WeeeReused>(returnId);
            var weeeReusedAmounts = new List<WeeeReusedAmount>();
            var weeeReusedSites = new List<WeeeReusedSite>();
            var addresses = new List<AatfAddress>();
            foreach (var weeeReused in weeeReuseds)
            {
                weeeReusedAmounts.AddRange((await dataAccess.GetAll<WeeeReusedAmount>()).Where(w => w.WeeeReused.Id == weeeReused.Id));
                weeeReusedSites.AddRange((await dataAccess.GetAll<WeeeReusedSite>()).Where(w => w.WeeeReused.Id == weeeReused.Id));
            }

            foreach (var weeeReusedSite in weeeReusedSites)
            {
                addresses.Add(weeeReusedSite.Address);
            }

            dataAccess.RemoveMany<WeeeReusedSite>(weeeReusedSites);
            dataAccess.RemoveMany<AatfAddress>(addresses);
            dataAccess.RemoveMany<WeeeReusedAmount>(weeeReusedAmounts);
            dataAccess.RemoveMany<WeeeReused>(weeeReuseds);
        }

        private async void DeleteWeeeSentOnData(Guid returnId)
        {
            var weeeSentOns = await dataAccess.GetManyByReturnId<WeeeSentOn>(returnId);
            var weeeSentOnAmounts = new List<WeeeSentOnAmount>();
            var weeeSentOnSites = new List<Guid>();
            var addresses = new List<AatfAddress>();
            foreach (var weeeSentOn in weeeSentOns)
            {
                weeeSentOnAmounts.AddRange((await dataAccess.GetAll<WeeeSentOnAmount>()).Where(w => w.WeeeSentOn.Id == weeeSentOn.Id));
                addresses.Add(weeeSentOn.OperatorAddress);
                addresses.Add(weeeSentOn.SiteAddress);
            }

            dataAccess.RemoveMany<AatfAddress>(addresses);
            dataAccess.RemoveMany<WeeeSentOnAmount>(weeeSentOnAmounts);
            dataAccess.RemoveMany<WeeeSentOn>(weeeSentOns);
        }

        private async void DeleteNonObligatedData(Guid returnId)
        {
            var nonObligatedWeees = await dataAccess.GetManyByReturnId<NonObligatedWeee>(returnId);
            dataAccess.RemoveMany<NonObligatedWeee>(nonObligatedWeees);
        }
    }
}