namespace EA.Weee.Web.Extensions
{
    using System.Linq;
    using Core.Admin;
    using Core.DataReturns;
    using Core.Shared;

    public static class ProducerEeeDetailsExtensions
    {
        public static string DisplayTonnage(this ProducerEeeDetails producerEeeDetails, QuarterSelection quarter, WeeeCategory category,
            ObligationType obligationType)
        {
            switch (quarter)
            {
                case QuarterSelection.All:
                    var totalEee = producerEeeDetails.TotalEee.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return totalEee != null
                        ? totalEee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q1:
                    var quarter1Eee = producerEeeDetails.Q1EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter1Eee != null
                        ? quarter1Eee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q2:
                    var quarter2Eee = producerEeeDetails.Q2EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter2Eee != null
                        ? quarter2Eee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q3:
                    var quarter3Eee = producerEeeDetails.Q3EEE.SingleOrDefault(e => e.Category == category
                        && e.ObligationType == obligationType);
                    return quarter3Eee != null
                        ? quarter3Eee.Tonnage.ToString()
                        : string.Empty;

                case QuarterSelection.Q4:
                    var quarter4Eee = producerEeeDetails.Q4EEE.SingleOrDefault(e => e.Category == category
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