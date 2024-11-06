namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class OrganisationDetailsTabsViewModelTests
    {
        [Fact]
        public void OrganisationDetails_IsRegisteredIsCorrect()
        {
            var model = new OrganisationDetailsTabsViewModel();
            model.OrganisationViewModel = new OrganisationViewModel { };

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted;
            model.HasPaid = true;

            model.IsRegistered.Should().Be(true);

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.InComplete;
            model.HasPaid = true;

            model.IsRegistered.Should().Be(false);

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.InComplete;
            model.HasPaid = false;

            model.IsRegistered.Should().Be(false);
        }

        [Fact]
        public void OrganisationDetails_ShowReturnRegistrationToUserIsCorrect()
        {
            var model = new OrganisationDetailsTabsViewModel();
            model.OrganisationViewModel = new OrganisationViewModel { };

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted;
            model.HasPaid = true;
            model.IsInternal = true;
            model.IsAdmin = true;

            model.ShowReturnRegistrationToUser.Should().Be(true);

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted;
            model.HasPaid = false;
            model.IsInternal = true;
            model.IsAdmin = true;

            model.ShowReturnRegistrationToUser.Should().Be(true);

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted;
            model.HasPaid = true;
            model.IsInternal = false;
            model.IsAdmin = true;

            model.ShowReturnRegistrationToUser.Should().Be(false);

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted;
            model.HasPaid = false;
            model.IsInternal = true;
            model.IsAdmin = true;

            model.ShowReturnRegistrationToUser.Should().Be(true);

            model.Status = EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted;
            model.HasPaid = true;
            model.IsInternal = true;
            model.IsAdmin = false;

            model.ShowReturnRegistrationToUser.Should().Be(false);
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public void TabView_HidesShowsRemoveButton(bool isAdmin, bool isInternal, bool expected)
        {
            var model = new OrganisationDetailsTabsViewModel();
            model.OrganisationViewModel = new OrganisationViewModel { };

            model.IsAdmin = isAdmin;
            model.IsInternal = isInternal;

            model.IsInternalAdmin.Should().Be(expected);
        }
    }
}
