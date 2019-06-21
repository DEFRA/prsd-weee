namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Organisations;
    using Prsd.Core;

    public class SchemeDataList
    {
        public SchemeDataList()
        {
        }

        public SchemeDataList(IList<SchemeData> schemeData, OrganisationData organisationData)
        {
            Guard.ArgumentNotNull(() => schemeData, schemeData);
            Guard.ArgumentNotNull(() => organisationData, organisationData);

            SchemeDataItems = schemeData;
            OrganisationData = organisationData;
        }

        public virtual IList<SchemeData> SchemeDataItems { get; set; }

        public virtual OrganisationData OrganisationData { get; set; }
    }
}
