namespace EA.Weee.Core.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A DTO providing information about a scheme that is publically available.
    /// </summary>
    public class SchemePublicInfo
    {
        public Guid SchemeId { get; set; }
        
        public Guid OrganisationId { get; set; }
        
        public string Name { get; set; }
        
        public string ApprovalNo { get; set; }

        public string StatusName { get; set; }
    }
}
