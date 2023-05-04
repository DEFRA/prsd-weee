﻿namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Base;
    using Core.Helpers;
    using Core.Shared;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Weee.Tests.Core;

    public class AatfDbSetup : DbTestDataBuilder<Aatf, AatfDbSetup>
    {
        protected override Aatf Instantiate()
        {
            var auth = DbContext.UKCompetentAuthorities.First(c => c.Name.Equals("Environment Agency"));
            var localArea = DbContext.LocalAreas.First(c => c.Name.Equals("Kent, South London and East Sussex (KSL)"));
            var panArea = DbContext.PanAreas.First(c => c.Name.Equals("South East"));
            var address = AatfAddressDbSetup.Init().Create();
            var contact = AatfContactDbSetup.Init().Create();

            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var newAddress = DbContext.AatfAddress.First(a => a.Id.Equals(address.Id));
            var newContact = DbContext.AatfContacts.First(c => c.Id.Equals(contact.Id));
            var name = Faker.Company.Name();

            instance = new Aatf(name.Substring(0, name.Length - 1).Replace(",", string.Empty),
                auth,
                $"WEE/AA{Faker.RandomNumber.Next(1000, 9999)}ZZ/ATF",
                AatfStatus.Approved,
                organisation,
                newAddress,
                AatfSize.Large,
                DateTime.Now,
                newContact,
                FacilityType.Aatf,
                DateTime.Now.Year,
                localArea,
                panArea);

            return instance;
        }

        public AatfDbSetup WithOrganisation(Guid organisationId)
        {
            ObjectInstantiator<Aatf>.SetProperty(o => o.Organisation, null, instance);
            ObjectInstantiator<Aatf>.SetProperty(o => o.OrganisationId, organisationId, instance);

            return this;
        }

        public AatfDbSetup WithAppropriateAuthority(CompetentAuthority authority)
        {
            var authString = authority.ToDisplayString();
            var ca = DbContext.UKCompetentAuthorities.First(c => c.Name.Equals(authString));
            
            ObjectInstantiator<Aatf>.SetProperty(o => o.CompetentAuthority, null, instance);
            ObjectInstantiator<Aatf>.SetProperty(o => o.CompetentAuthority, ca, instance);

            return this;
        }

        public AatfDbSetup WithComplianceYear(Int16 complianceYear)
        {
            ObjectInstantiator<Aatf>.SetProperty((o) => o.ComplianceYear, complianceYear, instance);

            return this;
        }
    }
}
