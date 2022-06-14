namespace EA.Weee.Integration.Tests.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Domain.Obligation;

    public class ObligationUploadDbSetup : DbTestDataBuilder<ObligationUpload, ObligationUploadDbSetup>
    {
        protected override ObligationUpload Instantiate()
        {
            var auth = DbContext.UKCompetentAuthorities.First(c => c.Name.Equals("Environment Agency"));
            var user = DbContext.GetCurrentUser();
            instance = new ObligationUpload(auth, user, "file", "filename");

            return instance;
        }

        public ObligationUploadDbSetup WithErrors(List<ObligationUploadError> obligationUploadErrors)
        {
            instance.SetErrors(obligationUploadErrors);

            return this;
        }
    }
}
