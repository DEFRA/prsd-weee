namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;

    public class AddReturnReportOnHandler : IRequestHandler<AddReturnReportOn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGetAatfsDataAccess getAatfsDataAccess;

        public AddReturnReportOnHandler(IWeeeAuthorization authorization,
                                        IGenericDataAccess dataAccess,
                                        WeeeContext context,
                                        IWeeeSentOnDataAccess sentOnDataAccess,
                                        IGetAatfsDataAccess getAatfsDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.context = context;
            this.sentOnDataAccess = sentOnDataAccess;
            this.getAatfsDataAccess = getAatfsDataAccess;
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
                            await DeleteWeeeReceivedData(message.ReturnId);
                            break;

                        case (int)ReportOnQuestionEnum.WeeeSentOn:
                            await DeleteWeeeSentOnData(message.ReturnId);
                            break;

                        case (int)ReportOnQuestionEnum.WeeeReused:
                            await DeleteWeeeReusedData(message.ReturnId);
                            break;

                        case (int)ReportOnQuestionEnum.NonObligated:
                            await DeleteNonObligatedData(message.ReturnId, false);
                            break;

                        case (int)ReportOnQuestionEnum.NonObligatedDcf:
                            await DeleteNonObligatedData(message.ReturnId, true);
                            break;

                        default:
                            break;
                    }
                }
            }

            var oldReturnOptions = await dataAccess.GetManyByReturnId<ReturnReportOn>(message.ReturnId);
            dataAccess.RemoveMany(oldReturnOptions);

            await context.SaveChangesAsync();

            if (message.SelectedOptions != null && message.SelectedOptions.Any())
            {
                var returnReportOn = new List<ReturnReportOn>();

                foreach (var option in message.SelectedOptions)
                {
                    returnReportOn.Add(new ReturnReportOn(message.ReturnId, option));
                }

                if (message.DcfSelectedValue)
                {
                    var isParentSelected = message.SelectedOptions.Contains((int?)ReportOnQuestionEnum.NonObligated ?? default(int));

                    if (isParentSelected)
                    {
                        returnReportOn.Add(new ReturnReportOn(message.ReturnId, (int)ReportOnQuestionEnum.NonObligatedDcf));
                    }
                }

                await dataAccess.AddMany(returnReportOn);
            }
            return true;
        }

        private async Task DeleteWeeeReceivedData(Guid returnId)
        {
            var weeeReceiveds = await dataAccess.GetManyByReturnId<WeeeReceived>(returnId);
            var weeeReceivedAmounts = new List<WeeeReceivedAmount>();
            var weeeReturnSchemes = await dataAccess.GetManyByReturnId<ReturnScheme>(returnId);

            foreach (var weeeReceived in weeeReceiveds)
            {
                weeeReceivedAmounts.AddRange(await dataAccess.GetManyByExpression(new WeeeReceivedAmountByWeeeReceivedIdSpecification(weeeReceived.Id)));
            }

            dataAccess.RemoveMany(weeeReceivedAmounts);
            dataAccess.RemoveMany(weeeReceiveds);
            dataAccess.RemoveMany(weeeReturnSchemes);

            await context.SaveChangesAsync();
        }

        private async Task DeleteWeeeSentOnData(Guid returnId)
        {
            var weeeSentOns = await dataAccess.GetManyByReturnId<WeeeSentOn>(returnId);
            var weeeSentOnAmounts = new List<WeeeSentOnAmount>();
            var addresses = new List<AatfAddress>();

            foreach (var weeeSentOn in weeeSentOns)
            {
                weeeSentOnAmounts.AddRange(await dataAccess.GetManyByExpression(new WeeeSentOnAmountByWeeeSentOnIdSpecification(weeeSentOn.Id)));

                var weeeSentOnSiteCount = await sentOnDataAccess.GetWeeeSentOnBySiteId(weeeSentOn.SiteAddressId);
                if (weeeSentOnSiteCount == 1)
                {
                    var aafts = await getAatfsDataAccess.GetAatfsBySiteAddressId(weeeSentOn.SiteAddressId);
                    if (aafts.Count() == 0 && weeeSentOn.SiteAddress != null)
                    {
                        addresses.Add(weeeSentOn.SiteAddress);
                    }
                }

                var weeeSentOnOperatorCount = await sentOnDataAccess.GetWeeeSentOnByOperatorId(weeeSentOn.OperatorAddressId);
                if (weeeSentOnOperatorCount == 1)
                {
                    if (weeeSentOn.OperatorAddress != null)
                    {
                        addresses.Add(weeeSentOn.OperatorAddress);
                    }
                }
            }

            dataAccess.RemoveMany(addresses);
            dataAccess.RemoveMany(weeeSentOnAmounts);
            dataAccess.RemoveMany(weeeSentOns);

            await context.SaveChangesAsync();
        }

        private async Task DeleteWeeeReusedData(Guid returnId)
        {
            var weeeReuseds = await dataAccess.GetManyByReturnId<WeeeReused>(returnId);
            var weeeReusedAmounts = new List<WeeeReusedAmount>();
            var weeeReusedSites = new List<WeeeReusedSite>();
            var addresses = new List<AatfAddress>();
            foreach (var weeeReused in weeeReuseds)
            {
                weeeReusedAmounts.AddRange(await dataAccess.GetManyByExpression(new WeeeReusedAmountByWeeeReusedIdSpecification(weeeReused.Id)));
                weeeReusedSites.AddRange(await dataAccess.GetManyByExpression(new WeeeReusedSiteByWeeeReusedIdSpecification(weeeReused.Id)));
            }

            foreach (var weeeReusedSite in weeeReusedSites)
            {
                addresses.Add(weeeReusedSite.Address);
            }

            dataAccess.RemoveMany(weeeReusedSites);
            dataAccess.RemoveMany(addresses);
            dataAccess.RemoveMany(weeeReusedAmounts);
            dataAccess.RemoveMany(weeeReuseds);

            await context.SaveChangesAsync();
        }

        private async Task DeleteNonObligatedData(Guid returnId, bool dcf)
        {
            var nonObligatedWeees = await dataAccess.GetManyByReturnId<NonObligatedWeee>(returnId);

            if (dcf)
            {
                nonObligatedWeees = nonObligatedWeees.Where(n => n.Dcf).ToList();
            }

            dataAccess.RemoveMany(nonObligatedWeees);

            await context.SaveChangesAsync();
        }
    }
}