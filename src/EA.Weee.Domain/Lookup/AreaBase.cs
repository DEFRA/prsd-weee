namespace EA.Weee.Domain.Lookup
{
    using System;

    public class AreaBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CompetentAuthorityId { get; set; }
    }
}
