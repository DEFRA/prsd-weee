namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CopyPreviousQuarterAatfDataHandler : IRequestHandler<CopyPreviousQuarterAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IObligatedSentOnDataAccess obligatedSentOnDataAccess;

        public CopyPreviousQuarterAatfDataHandler(IWeeeAuthorization authorization,
                                                  IReturnDataAccess returnDataAccess,
                                                  IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess,
                                                  IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess,
                                                  IObligatedSentOnDataAccess obligatedSentOnDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.fetchWeeeSentOnAmountDataAccess = fetchWeeeSentOnAmountDataAccess;
            this.obligatedSentOnDataAccess = obligatedSentOnDataAccess;
        }

        public async Task<bool> HandleAsync(CopyPreviousQuarterAatf copyPreviousQuarterAatf)
        {
            authorization.EnsureCanAccessExternalArea();
            var weeeSentOnList = new List<WeeeSentOn>();
            var aatfReturnData = await returnDataAccess.GetById(copyPreviousQuarterAatf.ReturnId);
            if (aatfReturnData != null)
            {
                int currentAatfQuater = (int)aatfReturnData.Quarter.Q;
                int currentAatfYear = aatfReturnData.Quarter.Year;

                int copyAatfYQuarter = currentAatfQuater;
                int copyAatfYear = currentAatfYear;

                if (currentAatfQuater == 1)
                {
                    copyAatfYear = currentAatfYear - 1;
                    copyAatfYQuarter = 4;
                }
                else
                {
                    copyAatfYQuarter = currentAatfQuater - 1;
                }                
                
                var returnData = await returnDataAccess.GetByYearAndQuarter(copyPreviousQuarterAatf.OrganisationId, copyAatfYear, copyAatfYQuarter);
                if (returnData != null)
                {
                    var previousQuarterWeeeSentOnList = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(copyPreviousQuarterAatf.AatfId, returnData.Id);

                    if (copyPreviousQuarterAatf.IsPreviousQuarterDataCheck)
                    {
                        if (previousQuarterWeeeSentOnList != null && previousQuarterWeeeSentOnList.Count > 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        foreach (var weeeSentOnItem in previousQuarterWeeeSentOnList)
                        {
                            var copyWeeeSentOn = new WeeeSentOn(copyPreviousQuarterAatf.ReturnId, copyPreviousQuarterAatf.AatfId, weeeSentOnItem.OperatorAddressId, weeeSentOnItem.SiteAddressId);
                            weeeSentOnList.Add(copyWeeeSentOn);
                        }

                        await getSentOnAatfSiteDataAccess.Submit(weeeSentOnList);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
