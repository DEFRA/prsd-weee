namespace EA.Weee.Api.Client.Entities
{
    public class ResendActivationEmailByUserIdRequest
    {
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string ActivationBaseUrl { get; set; }
    }
}
