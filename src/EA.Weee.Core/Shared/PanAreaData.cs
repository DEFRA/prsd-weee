namespace EA.Weee.Core.Shared
{
    using System;

    [Serializable]
    public class PanAreaData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CompetentAuthorityId { get; set; }
    }
}
