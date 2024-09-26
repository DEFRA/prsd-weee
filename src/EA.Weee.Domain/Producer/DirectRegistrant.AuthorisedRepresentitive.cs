namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;

    public partial class DirectRegistrant
    {
        public bool HasAuthorisedRepresentitive => AuthorisedRepresentative != null;

        public void AddOrUpdateAuthorisedRepresentitive(AuthorisedRepresentative authorisedRepresentative)
        {
            Guard.ArgumentNotNull(() => authorisedRepresentative, authorisedRepresentative);

            AuthorisedRepresentative = authorisedRepresentative.OverwriteWhereNull(AuthorisedRepresentative);
        }
    }
}
