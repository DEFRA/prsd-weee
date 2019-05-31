namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using System;
    using System.Linq;
    using EA.Weee.Core.Organisations;
    using FluentAssertions;
    using Xunit;

    public class OrganisationDataTests
    {
        [Fact]
        public void IsRegisteredCompany_GivenRegisteredCompany_TrueReturned()
        {
            var organisation = new OrganisationData()
            {
                OrganisationType = OrganisationType.RegisteredCompany
            };

            organisation.IsRegisteredCompany.Should().BeTrue();
        }

        [Fact]
        public void IsRegisteredCompany_GivenNotRegisteredCompany_FalseReturned()
        {
            foreach (var value in Enum.GetValues(typeof(OrganisationType)).Cast<int>().Where(c => c != (int)OrganisationType.RegisteredCompany))
            {
                var organisation = new OrganisationData()
                {
                    OrganisationType = (OrganisationType)value
                };                
            }
        }
    }
}
