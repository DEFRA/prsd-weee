namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Domain.Obligation;
    using Weee.Tests.Core;

    public class ObligationSchemeDbSetup : DbTestDataBuilder<ObligationScheme, ObligationSchemeDbSetup>
    {
        protected override ObligationScheme Instantiate()
        {
            var auth = DbContext.UKCompetentAuthorities.First(c => c.Name.Equals("Environment Agency"));
            var user = DbContext.GetCurrentUser();
            var scheme = DbContext.Schemes.First();
            instance = new ObligationScheme(scheme, 2022);

            return instance;
        }

        public ObligationSchemeDbSetup WithObligationAmounts(List<ObligationSchemeAmount> amounts)
        {
            instance.SetAmounts(amounts);

            return this;
        }

        public ObligationSchemeDbSetup WithScheme(Guid schemeId)
        {
            instance.UpdateScheme(schemeId);

            return this;
        }

        public ObligationSchemeDbSetup WithObligationUpload(Guid obligationUploadId)
        {
            instance.UpdateObligationUpload(obligationUploadId);

            return this;
        }

        public ObligationSchemeDbSetup WithComplianceYear(int year)
        {
            ObjectInstantiator<ObligationScheme>.SetProperty(o => o.ComplianceYear, year, instance);

            return this;
        }
    }
}
