namespace EA.Weee.Api.Client.Entities
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.Validation;

    public class InternalUserCreationData : UserCreationData
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [InternalEmailAddress]
        public string Email { get; set; }
    }
}
