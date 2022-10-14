namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CopyPreviousQuarterAatfDataHandler : IRequestHandler<CopyPreviousQuarterAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public CopyPreviousQuarterAatfDataHandler(IWeeeAuthorization authorization,
                                                  IReturnDataAccess returnDataAccess,
                                                  IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess,
                                                  IOrganisationDetailsDataAccess organisationDetailsDataAccess,
                                                  IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
            this.genericDataAccess = genericDataAccess;
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

                int copyAatfQuarter = currentAatfQuater;
                int copyAatfYear = currentAatfYear;

                if (currentAatfQuater == 1)
                {
                    copyAatfYear = currentAatfYear - 1;
                    copyAatfQuarter = 4;
                }
                else
                {
                    copyAatfQuarter = currentAatfQuater - 1;
                }

                var returnData = await returnDataAccess.GetByYearAndQuarter(copyPreviousQuarterAatf.OrganisationId, copyAatfYear, copyAatfQuarter);
                if (returnData != null)
                {
                    var previousQuarterWeeeSentOnList = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(copyPreviousQuarterAatf.AatfId, returnData.Id);

                    if (copyPreviousQuarterAatf.IsPreviousQuarterDataCheck)
                    {
                        if (previousQuarterWeeeSentOnList != null && previousQuarterWeeeSentOnList.Count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        foreach (var weeeSentOnItem in previousQuarterWeeeSentOnList)
                        {
                            //The Operator Address is null then it is an AATF record
                            if (weeeSentOnItem != null && weeeSentOnItem.OperatorAddress != null)
                            {
                                var siteCountry = await organisationDetailsDataAccess.FetchCountryAsync(weeeSentOnItem.SiteAddress.CountryId);
                                var operatorCountry = await organisationDetailsDataAccess.FetchCountryAsync(weeeSentOnItem.OperatorAddress.CountryId);

                                var @return = await returnDataAccess.GetById(copyPreviousQuarterAatf.ReturnId);

                                var aatf = await genericDataAccess.GetById<Aatf>(copyPreviousQuarterAatf.AatfId);

                                var siteAddress = new AatfAddress(
                                    weeeSentOnItem.SiteAddress.Name,
                                    weeeSentOnItem.SiteAddress.Address1,
                                    weeeSentOnItem.SiteAddress.Address2,
                                    weeeSentOnItem.SiteAddress.TownOrCity,
                                    weeeSentOnItem.SiteAddress.CountyOrRegion,
                                    weeeSentOnItem.SiteAddress.Postcode,
                                    siteCountry);

                                var operatorAddress = new AatfAddress(
                                    weeeSentOnItem.OperatorAddress.Name,
                                    weeeSentOnItem.OperatorAddress.Address1,
                                    weeeSentOnItem.OperatorAddress.Address2,
                                    weeeSentOnItem.OperatorAddress.TownOrCity,
                                    weeeSentOnItem.OperatorAddress.CountyOrRegion,
                                    weeeSentOnItem.OperatorAddress.Postcode,
                                    operatorCountry);

                                var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);
                                weeeSentOnList.Add(weeeSentOn);
                            }
                            else
                            {
                                var copyWeeeSentOn = new WeeeSentOn(copyPreviousQuarterAatf.ReturnId, copyPreviousQuarterAatf.AatfId, weeeSentOnItem.OperatorAddressId, weeeSentOnItem.SiteAddressId);
                                weeeSentOnList.Add(copyWeeeSentOn);
                            }
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
