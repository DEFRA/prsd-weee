using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.DataReturns
{
    public class DataReturnError
    {
        public string Description { get; private set; }

        public DataReturnError(string description)
        {
            Description = description;
        }
    }
}
