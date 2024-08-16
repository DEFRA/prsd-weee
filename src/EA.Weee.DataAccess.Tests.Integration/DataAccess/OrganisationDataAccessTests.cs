namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using AutoFixture;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.User;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class OrganisationDataAccessTests
    {
        private readonly Fixture fixture;

        public OrganisationDataAccessTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async void DeleteOrganisation_GivenOrganisation_OrganisationShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var organisationDontRemove = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Organisations.Add(organisationDontRemove);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().NotBeEmpty();

                await organisationDataAccess.Delete(organisation.Id);

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisationDontRemove.Id).Should().NotBeEmpty();
            }
        }

        [Fact]
        public async void DeleteOrganisation_GivenOrganisationWithOrganisationUsers_OrganisationAndOrganisationUsersShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.OrganisationUsers.Add(
                    new Domain.Organisation.OrganisationUser(Guid.Parse(databaseWrapper.Model.AspNetUsers.First().Id), organisation.Id, UserStatus.Active));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.OrganisationUsers.Where(o => o.OrganisationId == organisation.Id).Should().NotBeEmpty();

                await organisationDataAccess.Delete(organisation.Id);

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.OrganisationUsers.Where(o => o.Id == organisation.Id).Should().BeEmpty();
            }
        }

        [Fact]
        public async void DeleteOrganisation_GivenInvalidOrganisation_ArgumentExceptionExpected()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var action = await Xunit.Record.ExceptionAsync(() => organisationDataAccess.Delete(Guid.Empty));

                action.Should().BeOfType<ArgumentException>();
            }
        }

        [Fact]
        public async void HasActiveUsers_GivenOrganisationWithNoUsers_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasActiveUsers(organisation.Id);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasActiveUsers_GivenOrganisationWithActiveUser_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var organisationUser = new Domain.Organisation.OrganisationUser(Guid.Parse(databaseWrapper.Model.AspNetUsers.First().Id),
                    organisation.Id, UserStatus.Active);
                databaseWrapper.WeeeContext.OrganisationUsers.Add(organisationUser);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasActiveUsers(organisation.Id);

                result.Should().BeTrue();
            }
        }

        public static IEnumerable<object[]> InactiveUserStatusData => new List<object[]>
        {
            new object[] {UserStatus.Pending},
            new object[] {UserStatus.Rejected},
            new object[] {UserStatus.Inactive}
        };

        [Theory]
        [MemberData(nameof(InactiveUserStatusData))]
        public async void HasActiveUsers_GivenOrganisationWithInactiveUser_FalseShouldBeReturned(UserStatus status)
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var organisationUser = new Domain.Organisation.OrganisationUser(Guid.Parse(databaseWrapper.Model.AspNetUsers.First().Id), organisation.Id, status);
                databaseWrapper.WeeeContext.OrganisationUsers.Add(organisationUser);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasActiveUsers(organisation.Id);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasReturns_GivenOrganisationHasReturnsForComplianceYear_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2019, QuarterType.Q1);

                databaseWrapper.WeeeContext.Returns.Add(@return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasReturns(organisation.Id, 2019);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HasReturns_GivenOrganisationDoesNotHaveReturnsForComplianceYear_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id,
                    FacilityType.Aatf, 2020, QuarterType.Q1);

                databaseWrapper.WeeeContext.Returns.Add(@return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasReturns(organisation.Id, 2019);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasFacility_GivenOrganisationDoesNotHaveAatf_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var ae = ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(ae);
                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasFacility(organisation.Id, FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasFacility_GivenOrganisationHasAatf_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);
                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasFacility(organisation.Id, FacilityType.Aatf);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async void HasFacility_GivenOrganisationHasAe_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var ae = ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(ae);
                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasFacility(organisation.Id, FacilityType.Ae);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async void HasFacility_GivenOrganisationDoesNotHavAe_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);
                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasFacility(organisation.Id, FacilityType.Ae);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasScheme_GivenOrganisationDoesNotHaveScheme_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasScheme(organisation.Id);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasScheme_GivenOrganisationDoesHasScheme_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Schemes.Add(new Domain.Scheme.Scheme(organisation));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasScheme(organisation.Id);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async void HasMultipleFacility_GivenOrganisationDoesNotHaveMultipleFacilityOfAatf_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, @organisation));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasMultipleOfEntityFacility(organisation.Id, FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasMultipleFacility_GivenOrganisationDoesNotHaveMultipleFacilityOfAe_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, @organisation));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasMultipleOfEntityFacility(organisation.Id, FacilityType.Ae);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasMultipleFacility_GivenOrganisationDoesFacilityTypeDoesNotMatch_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, @organisation));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasMultipleOfEntityFacility(organisation.Id, FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasMultipleFacility_GivenOrganisationDoesHaveMultipleFacilityOfAatf_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, @organisation));
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, @organisation));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasMultipleOfEntityFacility(organisation.Id, FacilityType.Aatf);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async void HasMultipleFacility_GivenOrganisationDoesHaveMultipleFacilityOfAe_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, @organisation));
                databaseWrapper.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAe(databaseWrapper, @organisation));

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasMultipleOfEntityFacility(organisation.Id, FacilityType.Ae);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GetReturnsByComplianceYear_GivenComplianceYearAndOrganisationAndFacilityType_ReturnsShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var @return1 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id);

                databaseWrapper.WeeeContext.Returns.Add(@return1);
                databaseWrapper.WeeeContext.Returns.Add(@return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.GetReturnsByComplianceYear(organisation.Id, 2019, FacilityType.Aatf);

                result.Should().Contain(@return1);
                result.Should().Contain(@return2);
            }
        }

        [Fact]
        public async Task GetReturnsByComplianceYear_GivenComplianceYearAndNonMatchingOrganisation_CorrectReturnsShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation1 = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var organisation2 = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var @return1 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation1, databaseWrapper.Model.AspNetUsers.First().Id);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation2, databaseWrapper.Model.AspNetUsers.First().Id);

                databaseWrapper.WeeeContext.Returns.Add(@return1);
                databaseWrapper.WeeeContext.Returns.Add(@return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.GetReturnsByComplianceYear(organisation1.Id, 2019, FacilityType.Aatf);

                result.Should().Contain(@return1);
                result.Should().NotContain(@return2);
            }
        }

        [Fact]
        public async Task GetReturnsByComplianceYear_GivenNonMatchingComplianceYearAndOrganisation_CorrectReturnsShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var @return1 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2020, QuarterType.Q1);

                databaseWrapper.WeeeContext.Returns.Add(@return1);
                databaseWrapper.WeeeContext.Returns.Add(@return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.GetReturnsByComplianceYear(organisation.Id, 2019, FacilityType.Aatf);

                result.Should().Contain(@return1);
                result.Should().NotContain(@return2);
            }
        }

        [Fact]
        public async Task GetReturnsByComplianceYear_GivenNonMatchingFacilityType_CorrectReturnsShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var @return1 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id);
                var @return2 = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id, FacilityType.Ae, 2019, QuarterType.Q1);

                databaseWrapper.WeeeContext.Returns.Add(@return1);
                databaseWrapper.WeeeContext.Returns.Add(@return2);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.GetReturnsByComplianceYear(organisation.Id, 2019, FacilityType.Aatf);

                result.Should().Contain(@return1);
                result.Should().NotContain(@return2);
            }
        }

        [Fact]
        public async Task GetReturnsByComplianceYear_GivenNonMatchingParameters_EmptyShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, databaseWrapper.Model.AspNetUsers.First().Id);

                databaseWrapper.WeeeContext.Returns.Add(@return);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.GetReturnsByComplianceYear(Guid.NewGuid(), 2020, FacilityType.Ae);

                result.Should().BeEmpty();
            }
        }
    }
}
