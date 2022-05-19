namespace EA.Weee.Web.Areas.Aatf.Comparers
{
    using System.Collections.Generic;
    using EA.Weee.Core.Scheme;

    public class SchemeDataComparer : IEqualityComparer<SchemeData>
    {
        public bool Equals(SchemeData x, SchemeData y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(SchemeData obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
