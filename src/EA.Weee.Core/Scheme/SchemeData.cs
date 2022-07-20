namespace EA.Weee.Core.Scheme
{
    using Organisations;
    using Shared;
    using System;

    [Serializable]
    public class SchemeData
    {
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public SchemeStatus SchemeStatus { get; set; }
        public string SchemeName { get; set; }

        public string SchemeNameDisplay => SchemeName ?? $"Empty name ({SchemeStatus.ToString()})";

        public string ApprovalName { get; set; }
        public string IbisCustomerReference { get; set; }
        public ObligationType? ObligationType { get; set; }
        public Guid? CompetentAuthorityId { get; set; }
        public UKCompetentAuthorityData CompetentAuthority { get; set; }
        public SchemeDataAvailability SchemeDataAvailability { get; set; }
        public bool CanEdit { get; set; }
        public bool Selected { get; set; }

        public AddressData Address { get; set; }

        public ContactData Contact { get; set; }

        public bool HasAddress { get; set; }

        public bool HasContact { get; set; }
    }
}
