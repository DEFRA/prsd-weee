namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    public class NonObligatedWeeeReceivedCsvData
    {
        public int Year { get; set; }

        public string Quarter { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string OrganisationName { get; set; }

        public string Category { get; set; }

        public decimal? TotalNonObligatedWeeeReceived { get; set; }

        public decimal? TotalNonObligatedWeeeReceivedFromDcf { get; set; }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }
    }
}
