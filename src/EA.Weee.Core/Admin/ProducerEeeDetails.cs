namespace EA.Weee.Core.Admin
{
    using System.Collections.Generic;
    using DataReturns;

    public class ProducerEeeDetails
    {
        public List<Eee> TotalEee { get; set; }

        public List<Eee> Q1EEE { get; set; }

        public List<Eee> Q2EEE { get; set; }

        public List<Eee> Q3EEE { get; set; }

        public List<Eee> Q4EEE { get; set; }

        public ProducerEeeDetails()
        {
            TotalEee = new List<Eee>();
            Q1EEE = new List<Eee>();
            Q2EEE = new List<Eee>();
            Q3EEE = new List<Eee>();
            Q4EEE = new List<Eee>();
        }
    }
}
