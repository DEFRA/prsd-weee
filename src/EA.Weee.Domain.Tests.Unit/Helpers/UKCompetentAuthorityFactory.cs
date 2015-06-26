namespace EA.Weee.Domain.Tests.Unit.Helpers
{
    using System;
    using Domain;

    public class UKCompetentAuthorityFactory
    {
        public static UKCompetentAuthority Create(Guid id, Country country)
        {
            var competentAuthority = ObjectInstantiator<UKCompetentAuthority>.CreateNew();
            ObjectInstantiator<UKCompetentAuthority>.SetProperty(x => x.Country, country, competentAuthority);
            ObjectInstantiator<UKCompetentAuthority>.SetProperty(x => x.Id, id, competentAuthority);
            return competentAuthority;
        }
    }
}
