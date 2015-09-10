namespace EA.Weee.RequestHandlers.Organisations.JoinOrganisation.DataAccess
{
    public class JoinOrganisationResult
    {
        public bool Successful { get; private set; }

        public string ErrorMessage { get; private set; }

        private JoinOrganisationResult(bool canJoinOrganisation, string errorMessage)
        {
            Successful = canJoinOrganisation;
            ErrorMessage = errorMessage;
        }

        public static JoinOrganisationResult Success()
        {
            return new JoinOrganisationResult(true, null);
        }

        public static JoinOrganisationResult Fail(string errorMessage)
        {
            return new JoinOrganisationResult(false, errorMessage);
        }
    }
}
