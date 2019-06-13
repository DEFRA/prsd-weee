namespace EA.Weee.Domain.Admin
{
    using System;

    public class LocalArea
    {
        protected LocalArea()
        {
        }

        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public Guid CompetentAuthorityId { get; protected set; }
    }
}
