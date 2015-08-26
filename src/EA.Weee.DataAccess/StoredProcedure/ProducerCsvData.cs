namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class maps to the results of [Producer].[spgCSVDataByOrganisationIdAndComplianceYear].
    /// </summary>
    public class ProducerCsvData
    {
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public string RegistrationNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string ChargeBand { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime DateAmended { get; set; }
        public string AuthorisedRepresentative { get; set; }
        public string OverseasProducer { get; set; }
    }
}
