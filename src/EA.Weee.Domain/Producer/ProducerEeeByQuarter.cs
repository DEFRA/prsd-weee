namespace EA.Weee.Domain.Producer
{
    using DataReturns;
    using System.Collections.Generic;

    public class ProducerEeeByQuarter
    {
        public Quarter Quarter { get; set; }

        public IEnumerable<EeeOutputAmount> Eee { get; set; }
    }
}
