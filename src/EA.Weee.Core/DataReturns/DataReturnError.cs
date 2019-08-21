namespace EA.Weee.Core.DataReturns
{
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
