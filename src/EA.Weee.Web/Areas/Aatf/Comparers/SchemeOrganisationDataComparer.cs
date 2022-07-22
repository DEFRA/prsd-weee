namespace EA.Weee.Web.Areas.Aatf.Comparers
{
    using System.Collections.Generic;
    using EA.Weee.Core.Scheme;

    public class SchemeOrganisationDataComparer : IEqualityComparer<OrganisationSchemeData>
    {
        public bool Equals(OrganisationSchemeData x, OrganisationSchemeData y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(OrganisationSchemeData obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
