namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataReturnWarning : IErrorOrWarning
    {
        public string Description { get; private set; }

        public string TypeName
        {
            get { return "Warning"; }
        }

        public DataReturnWarning(string description)
        {
            Description = description;
        }
    }
}
