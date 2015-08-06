namespace EA.Weee.Domain.Scheme
{
    using Prsd.Core.Domain;

    public class MemberUploadErrorType : Enumeration
    {
        public static readonly MemberUploadErrorType Schema = new MemberUploadErrorType(1, "Schema");
        public static readonly MemberUploadErrorType Business = new MemberUploadErrorType(2, "Business");

        private MemberUploadErrorType(int value, string displayName)
            : base(value, displayName)
        {
        }

        protected MemberUploadErrorType()
        {
        }
    }
}