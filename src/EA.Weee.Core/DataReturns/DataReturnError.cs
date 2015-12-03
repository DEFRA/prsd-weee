namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataReturnError : IErrorOrWarning
    {
        public string Description { get; private set; }

        public string TypeName
        {
            get { return "Error"; }
        }

        public DataReturnError(string description)
        {
            Description = description;
        }
    }
}
