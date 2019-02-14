namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.RequestHandlers.Admin.GetSchemes;

    public class GetSchemeRequestHandler : IRequestHandler<GetReturnScheme, PCSData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly Scheme.IGetSchemesDataAccess schemeDataAccess;
  
        public GetSchemeRequestHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess, Scheme.IGetSchemesDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<PCSData> HandleAsync(GetReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            List<Guid> listReturnIds = new List<Guid>();
            PCSData pcsListData = new PCSData();

            listReturnIds = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(message.ReturnId);

            foreach (var schemeId in listReturnIds)
            {
                var schemeList = await schemeDataAccess.GetCompleteSchemes(schemeId);

                foreach (var item in schemeList)
                {
                    {
                        pcsListData.SchemeId = item.Id;
                        pcsListData.PCSName = item.SchemeName;
                        pcsListData.ApprovalNumber = item.ApprovalNumber;
                    }
                }
            }
            return pcsListData;
        }
    }
}
