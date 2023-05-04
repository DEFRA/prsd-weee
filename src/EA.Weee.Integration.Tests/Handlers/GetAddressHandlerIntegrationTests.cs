namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Builders;
    using Domain;
    using Domain.Organisation;
    using Domain.User;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using AddressData = Core.Shared.AddressData;

    public class GetAddressHandlerIntegrationTest : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAddressWhereAddressExists : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> handler;
            private static AddressData result;
            private static Organisation organisation;
            private static Country country;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings()
                    .WithExternalUserAccess();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init()
                    .WithUserIdAndOrganisationId(UserId, organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(new GetAddress(organisation.BusinessAddress.Id, organisation.Id))).Result;

                country = Query.GetCountryById(result.CountryId);
            };

            private readonly It shouldReturnAddress = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnExpectedPropertyValues = () =>
            {
                result.Id.Should().Be(organisation.BusinessAddressId.Value);
                result.Address1.Should().Be(organisation.BusinessAddress.Address1);
                result.Address2.Should().Be(organisation.BusinessAddress.Address2);
                result.CountryName.Should().Be(country.Name);
                result.CountryId.Should().Be(organisation.BusinessAddress.CountryId);
                result.CountyOrRegion.Should().Be(organisation.BusinessAddress.CountyOrRegion);
                result.Email.Should().Be(organisation.BusinessAddress.Email);
                result.Postcode.Should().Be(organisation.BusinessAddress.Postcode);
                result.TownOrCity.Should().Be(organisation.BusinessAddress.TownOrCity);
                result.Telephone.Should().Be(organisation.BusinessAddress.Telephone);
            };
        }

        [Component]
        public class WhenIGetAddressWhereAddressDoesNotExist : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> handler;
            private static Organisation organisation;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings()
                    .WithExternalUserAccess();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init()
                    .WithUserIdAndOrganisationId(UserId, organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(new GetAddress(Guid.NewGuid(), organisation.Id)));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<ArgumentException>;
        }

        [Component]
        public class WhenIGetAddressWhereUserIsNotAuthorised : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> handler;
            private static Organisation organisation;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                organisation = OrganisationDbSetup.Init().Create();
                
                handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private readonly Because of = () =>
            { 
                CatchExceptionAsync(() => handler.HandleAsync(new GetAddress(organisation.BusinessAddress.Id, organisation.Id)));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }
    }
}
