namespace EA.Weee.Domain.Scheme
{
    using EA.Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MemberUploadRawData
    {
        [Key]
        public Guid MemberUploadId { get; set; }

        public string Data { get; set; }
    }
}
