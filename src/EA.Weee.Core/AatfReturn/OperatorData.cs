namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OperatorData
    {
        public OperatorData(Guid id, string operatorName)
        {
            this.Id = id;
            this.OperatorName = operatorName;
        }
        public Guid Id { get; set; }

        public string OperatorName { get; set; }
    }
}
