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

            // Case 3: External user, Submitted status (should not show continue registration)
            new object[] { false, false, SubmissionStatus.Submitted, 2024, 2024, false },

            // Case 4: External user, Returned status, matching year (should show continue registration)
            new object[] { false, false, SubmissionStatus.Returned, 2024, 2024, true },

            // Case 5: External user, Returned status, year does not match
            new object[] { false, false, SubmissionStatus.Returned, 2024, 2023, false },

            // Case 6: Internal admin, Incomplete status, matching year (should not show continue registration)
            new object[] { true, true, SubmissionStatus.InComplete, 2024, 2024, false },

            // Case 7: Internal admin, Returned status, matching year (should not show continue registration)
            new object[] { true, true, SubmissionStatus.Returned, 2024, 2024, false },

            // Case 8: Internal admin, Submitted status (should not show continue registration)
            new object[] { true, true, SubmissionStatus.Submitted, 2024, 2024, false },
        };

        [Theory]
        [MemberData(nameof(ShowReturnRegistrationToUserTestData))]
        public void ShowReturnRegistrationToUser_ReturnsExpectedValue(
                    bool isInternal,
                    bool isAdmin,
                    SubmissionStatus status,
                    bool hasPaid,
                    int currentYear,
                    int? year,
                    bool expectedResult)
        {
            // Arrange
            var model = CreateViewModel();
            model.IsInternal = isInternal;
            model.IsAdmin = isAdmin;
            model.Status = status;
            model.HasPaid = hasPaid;
            model.CurrentYear = currentYear;
            model.Year = year;

            // Act & Assert
            model.ShowReturnRegistrationToUser.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> ShowReturnRegistrationToUserTestData()
        {
            // Case 1: Internal admin, Registered (Submitted + Paid), matching year - should show
            yield return new object[] { true, true, SubmissionStatus.Submitted, true, 2024, 2024, true };

            // Case 2: Internal admin, Registered (Submitted + Paid), different year - should NOT show
            yield return new object[] { true, true, SubmissionStatus.Submitted, true, 2024, 2023, false };

            // Case 3: Internal admin, Submitted but not paid, matching year - should show
            yield return new object[] { true, true, SubmissionStatus.Submitted, false, 2024, 2024, true };

            // Case 4: Internal admin, Submitted but not paid, different year - should NOT show
            yield return new object[] { true, true, SubmissionStatus.Submitted, false, 2024, 2023, false };

            // Case 5: Not internal admin cases - should never show
            yield return new object[] { false, true, SubmissionStatus.Submitted, true, 2024, 2024, false };
            yield return new object[] { true, false, SubmissionStatus.Submitted, true, 2024, 2024, false };
            yield return new object[] { false, false, SubmissionStatus.Submitted, true, 2024, 2024, false };

            // Case 6: Internal admin, not submitted status - should not show
            yield return new object[] { true, true, SubmissionStatus.InComplete, false, 2024, 2024, false };
            yield return new object[] { true, true, SubmissionStatus.Returned, false, 2024, 2024, false };

            // Case 7: Internal admin, Year is null - should not show
            yield return new object[] { true, true, SubmissionStatus.Submitted, true, 2024, null, false };
        }

        private OrganisationDetailsTabsViewModel CreateViewModel()
        {
            return new OrganisationDetailsTabsViewModel
            {
                OrganisationViewModel = new OrganisationViewModel()
            };
        }
    }
}
