namespace EA.Weee.Core.Scheme
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SchemeDataAvailability
    {
        public IList<SchemeAnnualDataAvailability> AnnualDataAvailibilities { get; set; }

        public SchemeDataAvailability()
        {
            AnnualDataAvailibilities = new List<SchemeAnnualDataAvailability>();
        }
    }
}
