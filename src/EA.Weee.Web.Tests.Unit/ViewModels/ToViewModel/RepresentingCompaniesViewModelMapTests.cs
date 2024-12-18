namespace EA.Weee.Web.Tests.Unit.ViewModels.ToViewModel
{
    using AutoFixture;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.ViewModels.Organisation.Mapping.ToViewModel;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class RepresentingCompaniesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly RepresentingCompaniesViewModelMap mapper = new RepresentingCompaniesViewModelMap();

        [Fact]
        public void Map_ShouldSetOrganisationId()
        {
            // Arrange
            var source = TestFixture.Create<RepresentingCompaniesViewModelMapSource>();

            // Act
            var result = mapper.Map(source);

            // Assert
            result.OrganisationId.Should().Be(source.OrganisationData.Id);
        }

        [Fact]
        public void Map_ShouldSetShowBackButtonTrue_WhenMultipleActiveOrganisations()
        {
            // Arrange
            var source = new RepresentingCompaniesViewModelMapSource(
                new List<OrganisationUserData>
            {
                    new OrganisationUserData { UserStatus = UserStatus.Active },
                    new OrganisationUserData { UserStatus = UserStatus.Active }
            }, new OrganisationData());

            // Act
            var result = mapper.Map(source);

            // Assert
            result.ShowBackButton.Should().BeTrue();
        }

        [Fact]
        public void Map_ShouldSetShowBackButtonFalse_WhenSingleActiveOrganisation()
        {
            // Arrange
            var source = new RepresentingCompaniesViewModelMapSource(
                new List<OrganisationUserData>
                {
                        new OrganisationUserData { UserStatus = UserStatus.Active },
                        new OrganisationUserData { UserStatus = UserStatus.Inactive }
                }, new OrganisationData());
            // Act
            var result = mapper.Map(source);

            // Assert
            result.ShowBackButton.Should().BeFalse();
        }

        [Fact]
        public void Map_ShouldAddDirectRegistrantsWithNonEmptyRepresentedCompanyName()
        {
            // Arrange
            var directRegistrants = new List<DirectRegistrantInfo>
                {
                    new DirectRegistrantInfo { DirectRegistrantId = Guid.NewGuid(), RepresentedCompanyName = "Company A" },
                    new DirectRegistrantInfo { DirectRegistrantId = Guid.NewGuid(), RepresentedCompanyName = string.Empty },
                    new DirectRegistrantInfo { DirectRegistrantId = Guid.NewGuid(), RepresentedCompanyName = "Company B" },
                    new DirectRegistrantInfo { DirectRegistrantId = Guid.NewGuid(), RepresentedCompanyName = null }
                };

            var organisationUsers = TestFixture.CreateMany<OrganisationUserData>().ToList();

            var source =
                new RepresentingCompaniesViewModelMapSource(organisationUsers, new OrganisationData()
                {
                    DirectRegistrants = directRegistrants
                });

            // Act
            var result = mapper.Map(source);

            // Assert
            result.Organisations.Should().HaveCount(2);
            result.Organisations.Should().Contain(o => o.Name == "Company A");
            result.Organisations.Should().Contain(o => o.Name == "Company B");
        }
    }
}
