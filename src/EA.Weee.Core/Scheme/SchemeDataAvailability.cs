namespace EA.Weee.Core.Scheme
{
    using System.Collections.Generic;

    public class SchemeDataAvailability
    {
        public IList<SchemeAnnualDataAvailability> AnnualDataAvailibilities { get; set; }

        public SchemeDataAvailability()
        {
            AnnualDataAvailibilities = new List<SchemeAnnualDataAvailability>();
        }
    }
}
