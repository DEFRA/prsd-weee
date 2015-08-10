namespace EA.Weee.Requests.Scheme
{
    using EA.Prsd.Core.Mediator;

    public class VerifySchemeNameExists : IRequest<bool>
    {
        public string SchemeName { get; private set; }

        public VerifySchemeNameExists(string schemeName)
        {
            SchemeName = schemeName;
        }
    }
}
