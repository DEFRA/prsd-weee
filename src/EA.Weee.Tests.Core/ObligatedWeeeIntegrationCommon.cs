namespace EA.Weee.Tests.Core
{
    using System;
    using System.Linq;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.Tests.Core.Model;
    using Aatf = Domain.AatfReturn.Aatf;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;

    public static class ObligatedWeeeIntegrationCommon
    {
        public static Return CreateReturn(Organisation organisation, string createdById, FacilityType facilityType = null)
        {
            if (organisation == null)
            {
                organisation = Organisation.CreateSoleTrader("Test Organisation");
            }
            if (facilityType == null)
            {
                facilityType = FacilityType.Aatf;
            }

            return new Return(organisation, new Quarter(2019, QuarterType.Q1), createdById, facilityType);
        }

        public static Aatf CreateAatf(DatabaseWrapper database, Organisation organisation)
        {
            return new Aatf("aatfname", database.WeeeContext.UKCompetentAuthorities.First(), "number", AatfStatus.Approved, organisation, CreateAatfAddress(database), AatfSize.Large, DateTime.Now, CreateDefaultContact(database.WeeeContext.Countries.First()), FacilityType.Aatf, 2019, database.WeeeContext.LocalAreas.First(), database.WeeeContext.PanAreas.First());
        }

        public static Aatf CreateAe(DatabaseWrapper database, Organisation organisation)
        {
            return new Aatf("aatfname", database.WeeeContext.UKCompetentAuthorities.First(), "number", AatfStatus.Approved, organisation, CreateAatfAddress(database), AatfSize.Large, DateTime.Now, CreateDefaultContact(database.WeeeContext.Countries.First()), FacilityType.Ae, 2019, database.WeeeContext.LocalAreas.First(), database.WeeeContext.PanAreas.First());
        }

        public static Scheme CreateScheme(Organisation organisation)
        {
            return new Scheme(organisation);
        }

        public static AatfAddress CreateAatfAddress(DatabaseWrapper database)
        {
            return AddressHelper.GetAatfAddress(database);
        }

        public static Organisation CreateOrganisation()
        {
            var companyName = "Test Name" + Guid.NewGuid();
            var tradingName = "Test Trading Name" + Guid.NewGuid();
            const string companyRegistrationNumber = "ABC12345";

            var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);
            return organisation;
        }

        public static AatfContact CreateDefaultContact(Domain.Country country)
        {
            return new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");
        }
    }
}