namespace EA.Weee.Core.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides deatils about a producer (identified by their registration number)
    /// for a specific compliance year.
    /// </summary>
    public class ProducerDetails
    {
        public string RegistrationNumber { get; set; }
        
        public int ComplianceYear { get; set; }

        public List<ProducerDetailsScheme> Schemes { get; set; }
    }
}
