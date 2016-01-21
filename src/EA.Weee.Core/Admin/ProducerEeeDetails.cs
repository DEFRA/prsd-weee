namespace EA.Weee.Core.Admin
{
    using System.Collections.Generic;
    using System.Linq;
    using DataReturns;
    using Shared;

    public class ProducerEeeDetails
    {
        public List<Eee> TotalEEE { get; set; }

        public List<Eee> Q1EEE { get; set; }

        public List<Eee> Q2EEE { get; set; }

        public List<Eee> Q3EEE { get; set; }

        public List<Eee> Q4EEE { get; set; }

        public ProducerEeeDetails()
        {
            TotalEEE = new List<Eee>();
            Q1EEE = new List<Eee>();
            Q2EEE = new List<Eee>();
            Q3EEE = new List<Eee>();
            Q4EEE = new List<Eee>();
        }

        public string EeeByQuarterAndCategoryAndObligationType(QuarterSelection quarter, WeeeCategory category, ObligationType obligationType)
        {
            switch (quarter)
            {
                case QuarterSelection.All:
                    var totalEee = TotalEEE.SingleOrDefault(e => e.Category == category 
                        && e.ObligationType == obligationType);
                    return totalEee != null
                        ? totalEee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q1:
                    var quarter1Eee = Q1EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter1Eee != null
                        ? quarter1Eee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q2:
                    var quarter2Eee = Q2EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter2Eee != null
                        ? quarter2Eee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q3:
                    var quarter3Eee = Q3EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter3Eee != null
                        ? quarter3Eee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q4:
                    var quarter4Eee = Q4EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter4Eee != null
                        ? quarter4Eee.Tonnage.ToString()
                        : string.Empty;

                default:
                    return string.Empty;
            }
        }
    }
}
