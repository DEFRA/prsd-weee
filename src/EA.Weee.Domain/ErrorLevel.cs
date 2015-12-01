namespace EA.Weee.Domain
{
    using EA.Prsd.Core.Domain;

    public class ErrorLevel : Enumeration
    {
        public static readonly ErrorLevel Trace      = new ErrorLevel(1, "Trace");
        public static readonly ErrorLevel Debug      = new ErrorLevel(2, "Debug");
        public static readonly ErrorLevel Info       = new ErrorLevel(3, "Info");
        public static readonly ErrorLevel Warning    = new ErrorLevel(4, "Warning");
        public static readonly ErrorLevel Error      = new ErrorLevel(5, "Error");
        public static readonly ErrorLevel Fatal      = new ErrorLevel(6, "Fatal");

        private ErrorLevel(int value, string displayName)
            : base(value, displayName)
        {
        }

        protected ErrorLevel()
        {
        }
    }
}