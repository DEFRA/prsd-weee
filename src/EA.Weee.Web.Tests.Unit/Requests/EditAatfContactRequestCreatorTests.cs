namespace EA.Weee.Web.Tests.Unit.Requests
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Web.Requests;
    using EA.Weee.Web.ViewModels.Shared.Aatf;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class EditAatfContactRequestCreatorTests
    {
        private readonly IEditAatfContactRequestCreator requestCreator;

        public EditAatfContactRequestCreatorTests()
        {
            this.requestCreator = new EditAatfContactRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = new AatfEditContactAddressViewModel()
            {
                ContactData = A.Fake<AatfContactData>(),
                AatfData = A.Fake<AatfData>()
            };

            var request = this.requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_PropertiesShouldBeMapped()
        {
            var viewModel = new AatfEditContactAddressViewModel()
            {
                ContactData = A.Fake<AatfContactData>(),
                AatfData = A.Fake<AatfData>()
            };

            var request = this.requestCreator.ViewModelToRequest(viewModel);

            request.ContactData.Should().Be(viewModel.ContactData);
        }
    }
}
