namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers.Stubs
{
    using System.Collections.Generic;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.Admin.Controllers.Interfaces;

    public class GetAllYearsStub : IGetAllYears
    {
        private List<string> returnValue;

        public GetAllYearsStub(List<string> returnValue)
        {
            this.returnValue = returnValue;
        }

        public List<string> GetAllYears(IWeeeClient client)
        {
            return returnValue;
        }
    }
}
