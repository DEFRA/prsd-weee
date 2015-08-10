namespace EA.Weee.Core.Scheme
{
    using System;
    using MemberUploadTesting;
    using Shared;

    public class SchemeData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SchemeStatus SchemeStatus { get; set; }
        public string SchemeName { get; set; }
        public string IbisCustomerReference { get; set; }
        public ObligationType ObligationType { get; set; }
        public Guid CompetentAuthorityId { get; set; }
    }
}
