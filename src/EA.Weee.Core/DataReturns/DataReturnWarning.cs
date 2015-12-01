namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataReturnWarning
    {
        public string Description { get; private set; }

        public DataReturnWarning(string description)
        {
            Description = description;
        }
    }
}
