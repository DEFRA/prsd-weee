using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public enum SchemaVersion
    {
        [Display(Name = "3.04")]
        Version_3_04,

        [Display(Name = "3.06")]
        Version_3_06,
    }
}
