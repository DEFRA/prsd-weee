namespace EA.Weee.Domain.Producer
{
    using System.Collections.Generic;
    using DataReturns;

    public class ProducerEeeByQuarter
    {
        public Quarter Quarter { get; set; }

        public IEnumerable<EeeOutputAmount> Eee { get; set; }
    }
}
