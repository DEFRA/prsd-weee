namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.Scheme;

    internal interface IGetSchemeData
    {
        List<SchemeData> GetSchemeData();
    }
}
