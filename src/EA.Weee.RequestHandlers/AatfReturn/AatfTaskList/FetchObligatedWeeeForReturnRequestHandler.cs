namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.RequestHandlers.Security;
    using Request = Requests.AatfReturn.GetReturn;

    class FetchObligatedWeeeForReturnRequestHandler
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchObligatedWeeeForReturnDataAccess dataAccess;

        public FetchObligatedWeeeForReturnRequestHandler(
            IFetchObligatedWeeeForReturnDataAccess dataAccess, IWeeeAuthorization authdataaccess)
        {
            this.dataAccess = dataAccess;
            this.authorization = authdataaccess;
        }
    }
}
