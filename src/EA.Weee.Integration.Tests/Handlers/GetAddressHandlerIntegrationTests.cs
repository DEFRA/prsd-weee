namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Base.EA.Weee.Integration.Tests;
    using Builders;
    using Domain;
    using Domain.Organisation;
    using Domain.User;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using AddressData = Core.Shared.AddressData;

    
    public class GetAddressHandlerIntegrationTest : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAddressWhereAddressExists : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> _handler;
            private static AddressData _result;
            private static Organisation _organisation;
            private static Country _country;

            private Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                _organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init()
                    .WithUserIdAndOrganisationId(UserId, _organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                _handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private Because of = () =>
            {
                _result = Task.Run(async () => await _handler.HandleAsync(new GetAddress(_organisation.BusinessAddress.Id, _organisation.Id))).Result;

                _country = Query.GetCountryById(_result.CountryId);
            };

            private It shouldReturnAddress = () =>
            {
                _result.Should().NotBeNull();
            };

            private It shouldHaveReturnExpectedPropertyValues = () =>
            {
                _result.Id.Should().Be(_organisation.BusinessAddressId.Value);
                _result.Address1.Should().Be(_organisation.BusinessAddress.Address1);
                _result.Address2.Should().Be(_organisation.BusinessAddress.Address2);
                _result.CountryName.Should().Be(_country.Name);
                _result.CountryId.Should().Be(_organisation.BusinessAddress.CountryId);
                _result.CountyOrRegion.Should().Be(_organisation.BusinessAddress.CountyOrRegion);
                _result.Email.Should().Be(_organisation.BusinessAddress.Email);
                _result.Postcode.Should().Be(_organisation.BusinessAddress.Postcode);
                _result.TownOrCity.Should().Be(_organisation.BusinessAddress.TownOrCity);
                _result.Telephone.Should().Be(_organisation.BusinessAddress.Telephone);
            };

        }

        [Component]
        public class WhenIGetAddressWhereAddressDoesNotExist : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> _handler;
            private static Organisation _organisation;

            private Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                _organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init()
                    .WithUserIdAndOrganisationId(UserId, _organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                _handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private Because of = () =>
            {
                CatchExceptionAsync(() => _handler.HandleAsync(new GetAddress(Guid.NewGuid(), _organisation.Id)));
            };

            private It shouldHaveCaughtArgumentException = ShouldThrowException<ArgumentException>;
        }

        [Component]
        public class WhenIGetAddressWhereUserIsNotAuthorised : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> _handler;
            private static Organisation _organisation;

            private Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                _organisation = OrganisationDbSetup.Init().Create();
                
                _handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private Because of = () =>
            { 
                CatchExceptionAsync(() => _handler.HandleAsync(new GetAddress(_organisation.BusinessAddress.Id, _organisation.Id)));
            };

            private It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;

            private It shouldFail = () => true.Should().BeFalse();
        }
    }
}
