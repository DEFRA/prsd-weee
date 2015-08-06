namespace EA.Weee.Core.PCS
{
    using System;
    using Shared;

    public class PcsData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PcsStatus PcsStatus { get; set; }
    }
}
