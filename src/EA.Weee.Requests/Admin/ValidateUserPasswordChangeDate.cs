namespace EA.Weee.Requests.Admin
{
    using EA.Prsd.Core.Mediator;

    public class ValidateUserPasswordChangeDate : IRequest<bool>
    {
        public string UserId { get; set; }

        public ValidateUserPasswordChangeDate(string userId)
        {
            UserId = userId;
        }
    }
}
