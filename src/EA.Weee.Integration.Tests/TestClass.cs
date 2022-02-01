namespace EA.Weee.Integration.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Base.EA.Weee.Integration.Tests;
    using Builders;
    using Domain.Organisation;
    using Domain.User;
    using EA.Prsd.Core.Mediator;
    using FluentAssertions;
    using Requests.Organisations;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using AddressData = Core.Shared.AddressData;

    public class TestClass : IntegrationTestBase
    {
        [Component]
        public class WhenIStart : WeeeContextSpecification
        {
            private static IRequestHandler<GetAddress, AddressData> _handler;
            private static AddressData _result;
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

                AatfDbSetup.Init().Create();
                _handler = Container.Resolve<IRequestHandler<GetAddress, AddressData>>();
            };

            private Because of = () =>
            {
                _result = Task.Run(async () => await _handler.HandleAsync(new GetAddress(_organisation.BusinessAddress.Id, _organisation.Id))).Result;
            };

            private It shouldComplete = () =>
            {
                _result.Should().NotBeNull();
            };
        }
    }
}
