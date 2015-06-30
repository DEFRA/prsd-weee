namespace EA.Weee.Domain
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;

    public class MemberUpload : Entity
    {
        public string Data { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

        public MemberUpload(string data, List<MemberUploadError> errors)
        {
            Data = data;
            Errors = errors;
        }

        public MemberUpload(string data)
        {
            Data = data;
            Errors = new List<MemberUploadError>();
        }

        protected MemberUpload()
        {
        }
    }
}