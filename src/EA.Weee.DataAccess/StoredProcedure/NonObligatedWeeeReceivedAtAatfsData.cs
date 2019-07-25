namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    public class NonObligatedWeeeReceivedAtAatfsData
    {
        public string Authority { get; set; }

        public string PatArea { get; set; }
        public string Area { get; set; }

        public int Year { get; set; }

        public string Quarter { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string Aatf { get; set; }

        public string Category { get; set; }

        public decimal? TotalNonObligatedWeeeReceived { get; set; }

        public decimal? TotalNonObligatedWeeeReceivedFromDcf { get; set; }
    }
}
