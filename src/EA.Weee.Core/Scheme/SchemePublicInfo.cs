namespace EA.Weee.Core.Scheme
{
    using System;
    using Organisations;
    using Shared;

    /// <summary>
    /// A DTO providing information about a scheme that is publically available.
    /// </summary>
    public class SchemePublicInfo
    {
        public Guid SchemeId { get; set; }
        
        public Guid OrganisationId { get; set; }
        
        public virtual string Name { get; set; }
        
        public string ApprovalNo { get; set; }

        public string StatusName { get; set; }

        public AddressData Address { get; set; }

        public ContactData Contact { get; set; }
    }
}
