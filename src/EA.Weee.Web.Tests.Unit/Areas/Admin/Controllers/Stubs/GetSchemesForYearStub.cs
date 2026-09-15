namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers.Stubs
{
    using System.Collections.Generic;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Admin.Controllers.Interfaces;

    public partial class GetSchemesForYearStub : IGetSchemesForYear
    {
        private List<SchemeData> returnValue;

        public GetSchemesForYearStub(List<SchemeData> returnValue)
        {
            this.returnValue = returnValue;
        }

        public List<SchemeData> GetSchemesForYear(IWeeeClient client, int? selectedYear)
        {
            return returnValue;
        }
    }
}
