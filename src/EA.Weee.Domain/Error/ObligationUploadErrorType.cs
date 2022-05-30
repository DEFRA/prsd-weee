namespace EA.Weee.Domain.Error
{
    using Prsd.Core.Domain;

    public class ObligationUploadErrorType : Enumeration
    {
        public static readonly ObligationUploadErrorType File = new ObligationUploadErrorType(1, "File");
        public static readonly ObligationUploadErrorType Scheme = new ObligationUploadErrorType(2, "Scheme");
        public static readonly ObligationUploadErrorType Data = new ObligationUploadErrorType(2, "Data");

        private ObligationUploadErrorType(int value, string displayName)
            : base(value, displayName)
        {
        }

        protected ObligationUploadErrorType()
        {
        }
    }
}