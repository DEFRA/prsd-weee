namespace EA.Weee.Core.Admin
{
    using System;

    [Serializable]
    public class LocalAreaData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CompetentAuthorityId { get; set; }
    }
}