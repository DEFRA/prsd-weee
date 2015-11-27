namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class UploadErrorType : Enumeration
    {
        public static readonly UploadErrorType Schema = new UploadErrorType(1, "Schema");
        public static readonly UploadErrorType Business = new UploadErrorType(2, "Business");

        private UploadErrorType(int value, string displayName)
            : base(value, displayName)
        {
        }

        protected UploadErrorType()
        {
        }
    }
}