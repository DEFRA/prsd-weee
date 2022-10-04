namespace EA.Weee.Core.Shared
{
    using System;

    [Serializable]
    public class EntityIdDisplayNameData
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }
    }
}
