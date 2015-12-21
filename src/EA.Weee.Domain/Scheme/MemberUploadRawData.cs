namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class MemberUploadRawData
    {
        [Key]
        public Guid MemberUploadId { get; set; }

        public string Data { get; set; }
    }
}
