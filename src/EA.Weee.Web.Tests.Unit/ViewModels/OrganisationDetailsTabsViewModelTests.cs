namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(ShowPaymentLinkTestData))]
        public void ShowPaymentLink_ReturnsExpectedValue(
            bool isInternal,
            bool isAdmin,
            SubmissionStatus status,
            bool hasPaid,
            bool expectedResult)
        {
            // Arrange
            var model = CreateViewModel();
            model.IsInternal = isInternal;
            model.IsAdmin = isAdmin;
            model.Status = status;
            model.HasPaid = hasPaid;

            // Act & Assert
            model.ShowPaymentLink.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public void IsInternalAdmin_ReturnsExpectedValue(bool isAdmin, bool isInternal, bool expected)
        {
            // Arrange
            var model = CreateViewModel();
            model.IsAdmin = isAdmin;
            model.IsInternal = isInternal;

            // Act & Assert
            model.IsInternalAdmin.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ShowContinueRegistrationToUserTestData))]
        public void ShowContinueRegistrationToUser_ReturnsExpectedValue(
        bool isAdmin,
        bool isInternal,
        SubmissionStatus status,
        int currentYear,
        int year,
        bool expectedResult)
        {
            // Arrange
            var model = CreateViewModel();
            model.IsAdmin = isAdmin;
            model.IsInternal = isInternal;
            model.Status = status;
            model.CurrentYear = currentYear;
            model.Year = year;

            // Act & Assert
            model.ShowContinueRegistrationToUser.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> ShowPaymentLinkTestData()
        {
            yield return new object[] { true, true, SubmissionStatus.Submitted, false, true };  // Should show

            yield return new object[] { true, true, SubmissionStatus.Submitted, true, false };  // Already paid
            yield return new object[] { false, true, SubmissionStatus.Submitted, false, false }; // Not internal
            yield return new object[] { true, false, SubmissionStatus.Submitted, false, false }; // Not admin
            yield return new object[] { true, true, SubmissionStatus.InComplete, false, false }; // Not submitted
        }

        public static IEnumerable<object[]> ShowContinueRegistrationToUserTestData =>
        new List<object[]>
        {
            // isAdmin, isInternal, status, currentYear, year, expectedResult

            // Case 1: External user (not admin, not internal), Incomplete status, matching year
            new object[] { false, false, SubmissionStatus.InComplete, 2024, 2024, true },

            // Case 2: External user, Incomplete status, year does not match
            new object[] { false, false, SubmissionStatus.InComplete, 2024, 2023, false },

            // Case 3: External user, status is not InComplete
            new object[] { false, false, SubmissionStatus.Submitted, 2024, 2024, false },

            // Case 4: Internal admin, Incomplete status, matching year
            new object[] { true, true, SubmissionStatus.InComplete, 2024, 2024, false },

            // Case 5: Internal admin, Incomplete status, year does not match
            new object[] { true, true, SubmissionStatus.InComplete, 2024, 2023, false },

            // Case 6: Admin but not internal, Incomplete status, matching year (treated as external)
            new object[] { true, false, SubmissionStatus.InComplete, 2024, 2024, true },
        };

        private OrganisationDetailsTabsViewModel CreateViewModel()
        {
            return new OrganisationDetailsTabsViewModel
            {
                OrganisationViewModel = new OrganisationViewModel()
            };
        }
    }
}
