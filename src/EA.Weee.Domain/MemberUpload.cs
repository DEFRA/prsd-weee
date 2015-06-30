namespace EA.Weee.Domain
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;

    public class MemberUpload : Entity
    {
        public string Data { get; private set; }

        public List<string> Errors { get; private set; }

        public MemberUpload(string data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }
    }
}