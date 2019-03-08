﻿namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;

    public class SchemeDataList
    {
        public SchemeDataList()
        {
        }

        public SchemeDataList(IList<SchemeData> schemeData, OperatorData operatorData)
        {
            Guard.ArgumentNotNull(() => schemeData, schemeData);
            Guard.ArgumentNotNull(() => operatorData, operatorData);

            SchemeDataItems = schemeData;
            OperatorData = operatorData;
        }

        public virtual IList<SchemeData> SchemeDataItems { get; set; }

        public virtual OperatorData OperatorData { get; set; }
    }
}
