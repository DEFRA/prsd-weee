namespace EA.Prsd.Core.Autofac.Tests
{
    using System.Threading.Tasks;
    using global::Autofac;
    using Mediator;
    using Xunit;

    public class AutofacMediatorTests
    {
        private readonly AutofacMediator mediator;

        public AutofacMediatorTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<VoidRequestHandler>()
                .AsImplementedInterfaces();

            var container = builder.Build();

            mediator = new AutofacMediator(container.BeginLifetimeScope());
        }

        [Fact]
        public async Task CanResolveVoidRequestHandler()
        {
            var response = await mediator.SendAsync(new VoidRequest());

            Assert.Equal(Unit.Value, response);
        }

        private class VoidRequest : IRequest
        {
        }

        private class VoidRequestHandler : IRequestHandler<VoidRequest, Unit>
        {
            public Task<Unit> HandleAsync(VoidRequest message)
            {
                return Task.FromResult(Unit.Value);
            }
        }
    }
}