namespace EA.Weee.Core.DataReturns
{
    using System;

    public class DataReturnsUploadData
    {
        public Guid Id { get; set; }

        public int? ComplianceYear { get; set; }

        public bool IsSubmitted { get; set; }

        public Guid? SchemeId { get; set; }
    } 
}
