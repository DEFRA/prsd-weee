namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CopyPreviousQuarterAatfDataHandler : IRequestHandler<CopyPreviousQuarterAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;
        private readonly IObligatedSentOnDataAccess obligatedSentOnDataAccess;

        public CopyPreviousQuarterAatfDataHandler(IWeeeAuthorization authorization,
                                                  IReturnDataAccess returnDataAccess,
                                                  IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess,
                                                  IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess,
                                                  IMap<AatfAddress, AatfAddressData> addressMapper,
                                                  IObligatedSentOnDataAccess obligatedSentOnDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.fetchWeeeSentOnAmountDataAccess = fetchWeeeSentOnAmountDataAccess;
            this.addressMapper = addressMapper;
            this.obligatedSentOnDataAccess = obligatedSentOnDataAccess;
        }

        public async Task<bool> HandleAsync(CopyPreviousQuarterAatf message)
        {
            authorization.EnsureCanAccessExternalArea();
            var weeeSentOnList = new List<WeeeSentOn>();
            var aatfReturnData = await returnDataAccess.GetById(message.ReturnId);
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
                
                var returnData = await returnDataAccess.GetByYearAndQuarter(message.OrganisationId, copyAatfYear, copyAatfYQuarter);
                var previousQuarterWeeeSentOnList = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(message.AatfId, returnData.Id);

                foreach (var weeeSentOnItem in previousQuarterWeeeSentOnList)
                {
                    var copyWeeeSentOn = new WeeeSentOn(message.ReturnId, message.AatfId, weeeSentOnItem.OperatorAddressId, weeeSentOnItem.SiteAddressId);
                    weeeSentOnList.Add(copyWeeeSentOn);
                }

                await getSentOnAatfSiteDataAccess.Submit(weeeSentOnList);

                var currentQuarterAmounts = new List<WeeeSentOnAmount>();

                foreach (var weeeSentOnItem in previousQuarterWeeeSentOnList)
                {
                    var previousQuarterAmounts = await fetchWeeeSentOnAmountDataAccess.FetchObligatedWeeeSentOnForReturn(weeeSentOnItem.Id);
                    var currentCopyWeeeSentOn = weeeSentOnList.Find(x => x.OperatorAddressId == weeeSentOnItem.OperatorAddressId && x.SiteAddressId == weeeSentOnItem.SiteAddressId);
                    
                    foreach (var previousQuarterAmount in previousQuarterAmounts)
                    {
                        var weeeSentOnAmount = new WeeeSentOnAmount(currentCopyWeeeSentOn, previousQuarterAmount.CategoryId, previousQuarterAmount.HouseholdTonnage, previousQuarterAmount.NonHouseholdTonnage);
                        currentQuarterAmounts.Add(weeeSentOnAmount);
                    }                    
                }

                await obligatedSentOnDataAccess.Submit(currentQuarterAmounts);
            }            

            return true;
        }
    }
}
