namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetWeeeSentOnHandler : IRequestHandler<GetWeeeSentOn, List<WeeeSentOnData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess sentOnAmountDataAccess;
        private readonly IMap<WeeeSentOn, WeeeSentOnData> mapper;

        public GetWeeeSentOnHandler(IWeeeAuthorization authorization,
            ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess, IMap<WeeeSentOn, WeeeSentOnData> mapper, IFetchObligatedWeeeForReturnDataAccess sentOnAmountDataAccess)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.mapper = mapper;
            this.sentOnAmountDataAccess = sentOnAmountDataAccess;
        }

        public async Task<List<WeeeSentOnData>> HandleAsync(GetWeeeSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOnList = new List<WeeeSentOnData>();

            var weeeSentOn = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(message.AatfId, message.ReturnId);

            foreach (var item in weeeSentOn)
            {
                var amount = await sentOnAmountDataAccess.FetchObligatedWeeeSentOnForReturn(item.Id);
                var sentOn = mapper.Map(item);
                decimal b2b = 0.000m;
                decimal b2c = 0.000m;

                foreach (var tonnage in amount)
                {
                    if (tonnage.NonHouseholdTonnage != null)
                    {
                        b2b += (decimal)tonnage.NonHouseholdTonnage;
                    }

                    if (tonnage.HouseholdTonnage != null)
                    {
                        b2c += (decimal)tonnage.HouseholdTonnage;
                    }
                }

                var tonnages = new ObligatedCategoryValue()
                {
                    B2B = b2b.ToString(),
                    B2C = b2c.ToString()
                };

                sentOn.Tonnages = tonnages;
                weeeSentOnList.Add(sentOn);
            }

            return weeeSentOnList;
        }
    }
}
