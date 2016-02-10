namespace EA.Weee.Core.Scheme
{
    using System;
    using Shared;

    public class SchemeData
    {
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public SchemeStatus SchemeStatus { get; set; }
        public string SchemeName { get; set; }
        public string ApprovalName { get; set; }
        public string IbisCustomerReference { get; set; }
        public EA.Weee.Core.Shared.ObligationType? ObligationType { get; set; }
        public Guid? CompetentAuthorityId { get; set; }
        public UKCompetentAuthorityData CompetentAuthority { get; set; }
        public SchemeDataAvailability SchemeDataAvailability { get; set; }
        public bool CanEdit { get; set; }
    }
}
