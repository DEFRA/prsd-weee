namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [Producer].[spgCSVDataByOrganisationIdAndComplianceYear].
    /// </summary>
    public class ProducerCSVData
    {
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public string RegistrationNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string ChargeBand { get; set; }
        public string ObligationType { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime DateAmended { get; set; }
        public string AuthorisedRepresentative { get; set; }
        public string OverseasProducer { get; set; }
    }
}
