namespace EA.Weee.Api.Client.Entities
{
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InternalUserCreationData : UserCreationData
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [InternalEmailAddress]
        public string Email { get; set; }
    }
}
