namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class AuthorizedRepresentative
    {
        public OverseasProducer OverseasProducer { get; set; }

        public AuthorizedRepresentative()
        {
        }

        public static AuthorizedRepresentative Create(ISettings settings)
        {
            AuthorizedRepresentative authorizedRepresentative = new AuthorizedRepresentative();

            if (RandomHelper.OneIn(2))
            {
                authorizedRepresentative.OverseasProducer = OverseasProducer.Create(settings);
            }

            return authorizedRepresentative;
        }
    }
}
