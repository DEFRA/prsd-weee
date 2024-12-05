﻿namespace EA.Weee.Core.DataReturns
{
    public class DataReturnWarning : IErrorOrWarning
    {
        public string Description { get; private set; }

        public string TypeName => "Warning";

        public DataReturnWarning(string description)
        {
            Description = description;
        }
    }
}
