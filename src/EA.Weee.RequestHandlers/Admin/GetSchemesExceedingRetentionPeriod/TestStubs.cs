namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Z.EntityFramework.Extensions;

    public class GetSchemeDataStub : IGetSchemeData
    {
        private List<SchemeData> schemeData;
        public GetSchemeDataStub(List<SchemeData> schemeData)
        {
            this.schemeData = schemeData;
        }

        public List<SchemeData> GetSchemeData()
        {
            return schemeData;
        }
    }
}
