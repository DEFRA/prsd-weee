namespace EA.Weee.Web.Areas.Aatf.Comparers
{
    using System.Collections.Generic;
    using EA.Weee.Core.Shared;

    public class SchemeOrganisationDataComparer : IEqualityComparer<EntityIdDisplayNameData>
    {
        public bool Equals(EntityIdDisplayNameData x, EntityIdDisplayNameData y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(EntityIdDisplayNameData obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
