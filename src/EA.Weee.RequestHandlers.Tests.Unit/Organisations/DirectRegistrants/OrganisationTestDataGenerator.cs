namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Weee.Core.Organisations;
    using System.Collections.Generic;

    public class OrganisationTestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>();

        public OrganisationTestDataGenerator()
        {
            var externalTypes = new[] { ExternalOrganisationType.RegisteredCompany, ExternalOrganisationType.Partnership, ExternalOrganisationType.SoleTrader };
            var domainTypes = new[] { Domain.Organisation.OrganisationType.RegisteredCompany, Domain.Organisation.OrganisationType.DirectRegistrantPartnership, Domain.Organisation.OrganisationType.SoleTraderOrIndividual };
            var brandNameOptions = new[] { "SomeBrandName", null };
            var yesNoOptions = new YesNoType?[] { YesNoType.Yes, YesNoType.No, null };

            foreach (var externalType in externalTypes)
            {
                Domain.Organisation.OrganisationType organisationDomainType = null;
                switch (externalType)
                {
                    case ExternalOrganisationType.RegisteredCompany:
                        organisationDomainType = Domain.Organisation.OrganisationType.RegisteredCompany;
                        break;
                    case ExternalOrganisationType.Partnership:
                        organisationDomainType = Domain.Organisation.OrganisationType.DirectRegistrantPartnership;
                        break;
                    case ExternalOrganisationType.SoleTrader:
                        organisationDomainType = Domain.Organisation.OrganisationType.SoleTraderOrIndividual;
                        break;
                }

                foreach (var brandName in brandNameOptions)
                {
                    foreach (var yesNo in yesNoOptions)
                    {
                        data.Add(new object[] { externalType, organisationDomainType, brandName, yesNo });
                    }
                }
            }
        }

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
