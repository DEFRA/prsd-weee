namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Requests
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Web.Areas.Admin.Requests;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class EditAatfContactRequestCreatorTests
    {
        private readonly IEditAatfContactRequestCreator requestCreator;

        public EditAatfContactRequestCreatorTests()
        {
            requestCreator = new EditAatfContactRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = A.Fake<AatfEditContactAddressViewModel>();

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_PropertiesShouldBeMapped()
        {
            var viewModel = new AatfEditContactAddressViewModel()
            {
                ContactData = A.Fake<AatfContactData>(),
            };

            var request = requestCreator.ViewModelToRequest(viewModel) as EditAatfContact;

            request.ContactData.Should().Be(viewModel.ContactData);
        }
    }
}
