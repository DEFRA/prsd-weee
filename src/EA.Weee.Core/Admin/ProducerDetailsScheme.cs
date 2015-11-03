namespace EA.Weee.Core.Admin
{
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides scheme-specific details about a producer (identified by their
    /// registration number) for a specific compliance year.
    /// </summary>
    public class ProducerDetailsScheme
    {
        public string SchemeName { get; set; }
        
        public string ProducerName { get; set; }
        
        public string TradingName { get; set; }
        
        public string CompanyNumber { get; set; }
        
        public DateTime RegistrationDate { get; set; }
        
        public ObligationType ObligationType { get; set; }
        
        public ChargeBandType ChargeBandType { get; set; }

        public DateTime? CeasedToExist { get; set; }

        public string IsAuthorisedRepresentative { get; set; }
    }
}
