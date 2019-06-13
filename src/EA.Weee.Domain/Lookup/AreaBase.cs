namespace EA.Weee.Domain.Lookup
{
    using System;

    public class AreaBase
    {
        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public Guid CompetentAuthorityId { get; protected set; }
    }
}
