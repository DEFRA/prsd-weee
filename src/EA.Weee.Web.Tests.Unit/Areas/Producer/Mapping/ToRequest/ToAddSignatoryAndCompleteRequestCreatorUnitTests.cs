namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToRequest
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Mappings.ToRequest;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ToAddSignatoryAndCompleteRequestCreatorUnitTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly ToAddSignatoryAndCompleteRequestCreator requestCreator;

        public ToAddSignatoryAndCompleteRequestCreatorUnitTests()
        {
            mapper = A.Fake<IMapper>();
            requestCreator = new ToAddSignatoryAndCompleteRequestCreator(mapper);
        }

        [Fact]
        public void ViewModelToRequest_ShouldMapCorrectly()
        {
            // Arrange
            var viewModel = TestFixture.Create<AppropriateSignatoryViewModel>();

            // Act
            var result = requestCreator.ViewModelToRequest(viewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AddSignatoryAndCompleteRequest>();
            result.DirectRegistrantId.Should().Be(viewModel.DirectRegistrantId);
            result.ContactData.FirstName.Should().Be(viewModel.Contact.FirstName);
            result.ContactData.LastName.Should().Be(viewModel.Contact.LastName);
            result.ContactData.Position.Should().Be(viewModel.Contact.Position);
        }
    }
}