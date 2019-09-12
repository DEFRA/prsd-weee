namespace EA.Weee.Email.Tests.Unit.EventHandlers
{
    using System.Threading.Tasks;

    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Events;
    using EA.Weee.Email.EventHandlers;

    using FakeItEasy;

    using Xunit;

    public class ContactDetailsUpdateEventHandlerTests
    {
        private readonly ContactDetailsUpdateEventHandler handler;

        private readonly IWeeeEmailService emailService;

        public ContactDetailsUpdateEventHandlerTests()
        {
            emailService = A.Fake<IWeeeEmailService>();

            handler = new ContactDetailsUpdateEventHandler(this.emailService);
        }

        [Fact]
        public async Task HandleAsync_GivenEvent_EmailServiceShouldBeCalled()
        {
            var aatf = A.Fake<Aatf>();
            var @event = new AatfContactDetailsUpdateEvent(aatf);

            await this.handler.HandleAsync(@event);

            A.CallTo(
                () => this.emailService.SendOrganisationContactDetailsChanged(
                    @event.Aatf.CompetentAuthority.Email,
                    @event.Aatf.Name,
                    (EntityType)@event.Aatf.FacilityType.Value)).MustHaveHappenedOnceExactly();
        }
    }
}
