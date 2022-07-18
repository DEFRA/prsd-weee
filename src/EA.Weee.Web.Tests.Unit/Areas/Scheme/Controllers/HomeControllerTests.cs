namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using Api.Client;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Users;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using EA.Weee.Core.AatfReturn;

    using TestHelpers;
    using Web.Areas.Scheme.Controllers;
    using Web.Areas.Scheme.ViewModels;
    using Web.ViewModels.Shared.Scheme;
    using Web.ViewModels.Shared.Submission;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Users;
    using Weee.Requests.Users.GetManageableOrganisationUsers;
    using Xunit;

    using AddressData = EA.Weee.Core.Shared.AddressData;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async void GetChooseActivity_ChecksForValidityOfOrganisation()
        {
            try
            {
                await HomeController().ChooseActivity(A.Dummy<Guid>());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void GetChooseActivity_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ChooseActivity(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetChooseActivity_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ChooseActivity(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetActivities_DoesNotHaveMultipleOrganisationUsers_DoesNotReturnManageOrganisationUsersOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMultipleOrganisationUsers = false
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageOrganisationUsers, result);
        }

        [Fact]
        public async Task GetActivities_HasMultipleOrganisationUsers_ReturnsManageOrganisationUsersOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMultipleOrganisationUsers = true
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageOrganisationUsers, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableDataReturnsConfigurationSetToFalse_DoesNotReturnManageEeeWeeeDataOption()
        {
            var result = await HomeController(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageEeeWeeeData, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableDataReturnsConfigurationSetToTrue_ReturnsManageEeeWeeeDataOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = Guid.NewGuid()
              });

            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageEeeWeeeData, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableDataReturnsConfigurationSetToTrueAndEnabledPBSIsSetToFalse_ReturnsManageEeeWeeeDataOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = Guid.NewGuid()
              });

            var result = await HomeControllerSetupForEnableDataReturnsAndEnablePBSEvidenceNotes(true, false).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageEeeWeeeData, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableDataReturnsConfigurationSetToTrueAndEnablePBSEvidenceNotesIsSetToTrue_ShouldNotReturnManageEeeWeeeDataOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = Guid.NewGuid()
              });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = true
                });

            var result = await HomeControllerSetupForEnableDataReturnsAndEnablePBSEvidenceNotes(true, true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageEeeWeeeData, result);
        }

        [Fact]
        public async Task GetActivities_WithBalancingSchemeSetToDefaultValueOfFalse_ReturnsViewOrganisationDetailsOptions()
        {
            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewOrganisationDetails, result);
        }

        [Fact]
        public async Task GetActivities_WithEnabledPBSEvidenceNotesSetToTrueAndIsPBS_ShouldNotReturnViewOrganisationDetailsOptions()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = true
                });

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ViewOrganisationDetails, result);
        }

        [Fact]
        public async Task GetActivities_WithEnabledPBSEvidenceNotesSetToTrueAndIsNotPBS_ShouldReturnViewOrganisationDetailsOptions()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = false
                });

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewOrganisationDetails, result);
        }

        [Fact]
        public async Task GetActivities_HasScheme_ReturnsManagePcsMembersAndManageContactDetailsOptions()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = Guid.NewGuid()
              });

            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManagePcsMembers, result);
            Assert.Contains(PcsAction.ManagePcsContactDetails, result);
        }

        [Fact]
        public async Task GetActivities_HasSchemeAndIsBalancingSchemeIsDefaultSetToFalse_ReturnsManagePcsMembersAndManageContactDetailsOptions()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = Guid.NewGuid()
              });

            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManagePcsMembers, result);
            Assert.Contains(PcsAction.ManagePcsContactDetails, result);
        }

        [Fact]
        public async Task GetActivities_HasSchemeAndEnabledPBSEvidenceNotesIsTrue_ShoulNotReturnManagePcsMembersAndManageContactDetailsOptions()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = Guid.NewGuid(),
              });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = true
                });

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManagePcsMembers, result);
            Assert.DoesNotContain(PcsAction.ManagePcsContactDetails, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAnAatf_ReturnsAATFReturnOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFReturns(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageAatfReturns, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasNoAatf_DoesNotReturnsAATFReturnOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = false
              });

            var result = await HomeControllerSetupForAATFReturns(true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageAatfReturns, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToFalse_DoesNotReturnsAATFReturnOption()
        {
            var result = await HomeControllerSetupForAATFReturns(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageAatfReturns, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAnAatfAndEnabledPBSIsSetToFalse_ReturnsAATFReturnOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFReturnsAndPBSEvidenceNotes(true, false).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageAatfReturns, result);
        }
        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAnAatfAndEnabledPBSIsSetToFalse_ReturnsManageAatfContactDetailsOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFReturnsAndPBSEvidenceNotes(true, false).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageAatfContactDetails, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAesAndEnabledPBSIsSetToFalse_ReturnsManageAeReturnsOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAes = true
              });

            var result = await HomeControllerSetupForAATFReturnsAndPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageAeReturns, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAesAndEnablePBSEvidenceNotesIsSetToTrue_ShouldNotReturnManageAeReturnsOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAes = true
              });

            var result = await HomeControllerSetupForAATFEvidenceNotesAndPBSEvidenceNotes(true, true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageAeReturns, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAesAndEnabledPBSEvidenceNotesIsSetToFalse_ShouldNotReturnManageAeContactDetailsOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAes = true
              });
            
            var result = await HomeControllerSetupForAATFEvidenceNotesAndPBSEvidenceNotes(true, true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageAeContactDetails, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAnAatfAndEnablePBSEvidenceNotesIsSetToTrue_ShouldNotReturnAATFReturnOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFEvidenceNotesAndPBSEvidenceNotes(true, true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageAatfReturns, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrueAndOrganisationHasAnAatfAndEnablePBSEvidenceNotesIsSetToTrue_ShouldNotReturnAatfContactDetailsOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFEvidenceNotesAndPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageAatfContactDetails, result);
        }

        [Fact]
        public async Task GetActivities_DoesNotHaveMemberSubmissions_AndDoesNotHaveDataReturnSubmissions_DoesNotReturnViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = false,
                   HasDataReturnSubmissions = false
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasMemberSubmissions_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = true,
                   HasDataReturnSubmissions = false
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasDataReturnSubmissions_AndEnableDataReturnsConfigurationSetToTrue_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = false,
                   HasDataReturnSubmissions = true
               });

            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasMemberSubmissionsAndIsBalancingSchemeIsSetToFalse_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = true,
                   HasDataReturnSubmissions = false
               });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
            .Returns(new OrganisationData
                {
                    IsBalancingScheme = false
                });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasMemberSubmissionsAndEnabledPBSEvidenceNotesIsSetToTrue_ShouldNotReturnViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = true,
                   HasDataReturnSubmissions = false
               });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = true
                });

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasDataReturnSubmissions_AndEnableDataReturnsConfigurationSetToFalse_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = false,
                   HasDataReturnSubmissions = true
               });

            var result = await HomeController(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_WithIsBalancingSchemeSetToTrueAndEnablePBSEvidenceNotesSetToTrue_ShouldReturnsManagePBSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  IsBalancingScheme = true
              });

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManagePBSEvidenceNotes, result);
        }

        [Fact]
        public async Task GetActivities_WithIsBalancingSchemeSetToTrueAndEnablePBSEvidenceNotesSetToFalse_ShouldNotReturnsManagePBSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  IsBalancingScheme = true
              });

            var result = await HomeControllerSetupForPBSEvidenceNotes(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManagePBSEvidenceNotes, result);
        }

        [Fact]
        public async Task GetActivities_WithIsBalancingSchemeSetToFalseAndEnablePBSEvidenceNotesSetToTrue_ShouldNotReturnsManagePBSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  IsBalancingScheme = false
              });

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManagePBSEvidenceNotes, result);
        }

        [Fact]
        public async Task GetActivities_WithIsBalancingSchemeSetToFalseAndEnablePBSEvidenceNotesSetToFalse_ShouldNotReturnsManagePBSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  IsBalancingScheme = false
              });

            var result = await HomeControllerSetupForPBSEvidenceNotes(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManagePBSEvidenceNotes, result);
        }

        [Fact]
        public async Task GetActivities_WithIsBalancingSchemeSetToTrue_ShouldReturnsManageOrganisationUsersOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMultipleOrganisationUsers = true
               });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
            .Returns(new OrganisationData
            {
                IsBalancingScheme = true
            });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageOrganisationUsers, result);
        }

        [Fact]
        public async Task GetActivities_WithIsBalancingSchemeSetToFalse_ShouldReturnsManageOrganisationUsersOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMultipleOrganisationUsers = true
               });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
            .Returns(new OrganisationData
            {
                IsBalancingScheme = false
            });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageOrganisationUsers, result);
        }

        [Fact]
        public async void PostChooseActivity_ManagePcsMembersApprovedStatus_RedirectsToMemberRegistrationSummary()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Approved);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManagePcsMembers
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Summary", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ViewAATFContactDetails_RedirectsAatfToHomeControllerWithAatfFacilityType()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageAatfContactDetails
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
            Assert.Equal(FacilityType.Aatf, routeValues["facilityType"]);
        }

        [Fact]
        public async void PostChooseActivity_ViewAATFContactDetails_RedirectsAatfToHomeControllerWithAeFacilityType()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageAeContactDetails
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
            Assert.Equal(FacilityType.Ae, routeValues["facilityType"]);
        }

        [Fact]
        public async void PostChooseActivity_ManagePcsMembersPendingStatus_RedirectsToAuthorisationRequired()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Pending);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManagePcsMembers
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AuthorisationRequired", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Theory]
        [InlineData(SchemeStatus.Rejected)]
        [InlineData(SchemeStatus.Withdrawn)]
        public async void PostChooseActivity_ManagePcsMembersRejectedOrWithdrawnStatus_RedirectsToAuthorisationRequired(SchemeStatus status)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(status);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManagePcsMembers
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AuthorisationRequired", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManageOrganisationUsers_RedirectsToOrganisationUsersManagement()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageOrganisationUsers
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
        }

        [Fact]
        public async void PostChooseActivity_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageOrganisationUsers
            };
            var result = await controller.ChooseActivity(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void GetChooseSubmissionType_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ChooseSubmissionType(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetChooseSubmissionType_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ChooseSubmissionType(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostChooseSubmissionType_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.EeeOrWeeeDataReturns
            };
            var result = await controller.ChooseSubmissionType(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void PostChooseSubmissionType_MemberRegistrationsSelected_RedirectsToMemberRegistrationSubmissionHistory()
        {
            var result = await HomeController().ChooseSubmissionType(new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.MemberRegistrations
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async void PostChooseSubmissionType_DataReturnsSelected_RedirectsToDataReturnsSubmissionHistory()
        {
            var result = await HomeController().ChooseSubmissionType(new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.EeeOrWeeeDataReturns
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewDataReturnSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async void GetManageOrganisationUsers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ManageOrganisationUsers(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetManageOrganisationUsers_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetManageableOrganisationUsers>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData
                    {
                        UserId = Guid.NewGuid().ToString(),
                        UserStatus = UserStatus.Active,
                        User = new UserData()
                    },
                });

            var result = await HomeController().ManageOrganisationUsers(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ManageOrganisationUsers");
        }

        [Fact]
        public async void GetManageOrganisationUsers_NoUsers_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetManageableOrganisationUsers>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData(),
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() => HomeController().ManageOrganisationUsers(A.Dummy<Guid>()));
        }

        [Fact]
        public async void PostManageOrganisationUsers_ModalStateValid_ReturnsView()
        {
            var model = new OrganisationUsersViewModel
            {
                OrganisationUsers = new List<KeyValuePair<string, Guid>>
                {
                    new KeyValuePair<string, Guid>("User (UserStatus)", Guid.NewGuid()),
                    new KeyValuePair<string, Guid>("User (UserStatus)", Guid.NewGuid())
                },
                SelectedOrganisationUser = Guid.NewGuid()
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var result = await HomeController().ManageOrganisationUsers(A.Dummy<Guid>(), model);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUser", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
            Assert.True(isModelStateValid);
        }

        [Fact]
        public async void GetManageOrganisationUser_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ManageOrganisationUser(A.Dummy<Guid>(), A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetManageOrganisationUser_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationUser>._))
                .Returns(new OrganisationUserData
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserStatus = UserStatus.Active,
                    User = new UserData()
                });

            var result = await HomeController().ManageOrganisationUser(A.Dummy<Guid>(), A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ManageOrganisationUser");
        }

        [Fact]
        public async void GetManageOrganisationUser_NullOrganisationUserId_RedirectsToManageOrganisationUsers()
        {
            var result = await HomeController().ManageOrganisationUser(A.Dummy<Guid>(), (Guid?)null);

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostManageOrganisationUser_DoNotChangeSelected_MustNotHappendUpdateOrganisationUserStatus()
        {
            const string DoNotChange = "Do not change at this time";

            var model = new OrganisationUserViewModel
            {
                UserStatus = UserStatus.Active,
                UserId = Guid.NewGuid().ToString(),
                Firstname = "Test",
                Lastname = "Test",
                Username = "test@test.com",
                SelectedValue = DoNotChange,
                PossibleValues = new[] { "Inactive", DoNotChange }
            };

            await HomeController().ManageOrganisationUser(Guid.NewGuid(), model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationUserStatus>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void PostManageOrganisationUser_ModalStateValid_ReturnsView()
        {
            const string DoNotChange = "Do not change at this time";

            var model = new OrganisationUserViewModel
            {
                UserStatus = UserStatus.Active,
                UserId = Guid.NewGuid().ToString(),
                Firstname = "Test",
                Lastname = "Test",
                Username = "test@test.com",
                SelectedValue = "Inactive",
                PossibleValues = new[] { "Inactive", DoNotChange }
            };

            var result = await HomeController().ManageOrganisationUser(Guid.NewGuid(), model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationUserStatus>._))
                .MustHaveHappened(1, Times.Exactly);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ViewOrganisationDetails_RedirectsToViewOrganisationDetails()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewOrganisationDetails
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewOrganisationDetails", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithEnableDataReturnsConfigurationSetToFalse_RedirectsToViewSubmissionHistory()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = true,
                    HasDataReturnSubmissions = true
                });

            var result = await HomeController(false).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithNoSubmittedDataReturn_RedirectsToViewSubmissionHistory()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = true,
                    HasDataReturnSubmissions = false
                });

            var result = await HomeController(true).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithNoSubmittedMemberUpload_RedirectsToViewDataReturnSubmissionHistory()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = false,
                    HasDataReturnSubmissions = true
                });

            var result = await HomeController(true).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewDataReturnSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithSubmittedMemberUpload_AndSubmittedDataReturn_AndEnableDataReturnsConfigurationSetToTrue_RedirectsToChooseSubmissionType()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = true,
                    HasDataReturnSubmissions = true
                });

            var result = await HomeController(true).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseSubmissionType", routeValues["action"]);
        }

        [Fact]
        public async void GetViewOrganisationDetails_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ManageOrganisationUsers(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetViewOrganisationDetails_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await HomeController().ViewOrganisationDetails(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ViewOrganisationDetails");
        }

        [Fact]
        public void PostViewOrganisationDetails_RedirectToChooseActivityView()
        {
            var result = HomeController().ViewOrganisationDetails(A.Dummy<Guid>(), new ViewOrganisationDetailsViewModel());

            Assert.IsType<RedirectToRouteResult>(result);
            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseActivity", routeValues["action"]);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFEvidenceNotesConfigurationSetToTrueAndOrganisationHasAnAatf_ReturnsAATFEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            result.Should().Contain(PcsAction.ManageAatfEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFEvidenceNotesConfigurationSetToTrueAndOrganisationHasNoAatf_DoesNotReturnAATFEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = false
              });

            var result = await HomeControllerSetupForAATFEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            result.Should().NotContain(PcsAction.ManageAatfEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFEvidenceNotesConfigurationSetToFalseAndOrganisationHasAnAatf_DoesNotReturnsAATFEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true
              });

            var result = await HomeControllerSetupForAATFEvidenceNotes(false).GetActivities(A.Dummy<Guid>());

            result.Should().NotContain(PcsAction.ManageAatfEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFEvidenceNotesConfigurationSetToFalseAndOrganisationHasNoAatf_DoesNotReturnAATFEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = false
              });

            var result = await HomeControllerSetupForAATFEvidenceNotes(false).GetActivities(A.Dummy<Guid>());

            result.Should().NotContain(PcsAction.ManageAatfEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFEvidenceNotesConfigurationSetToTrueAndOrganisationHasAnAatfAndIsBalancingSchemeIsSetToFalse_ReturnsAATFEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true,
                  IsBalancingScheme = false
              });

            var result = await HomeControllerSetupForAATFEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            result.Should().Contain(PcsAction.ManageAatfEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFEvidenceNotesConfigurationSetToTrueAndOrganisationHasAnAatfAndEnabledPBSEvidenceNotesIsSetToTrue_ShouldNotReturnAATFEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  HasAatfs = true,
              });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = true
                });

            var result = await HomeControllerSetupForAATFEvidenceNotesAndPBSEvidenceNotes(true, true).GetActivities(A.Dummy<Guid>());

            result.Should().NotContain(PcsAction.ManageAatfEvidenceNotes);
        }

        [Fact]
        public async void PostChooseActivity_ManageAATFEvidenceNotes_RedirectsToManageAATFEvidenceNotes()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageAatfEvidenceNotes
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            // TODO - This is a place holder, please update to correct page once implemented
            routeValues["action"].Should().Be("Index");
        }

        //GC ------------------------------

        private HomeController HomeControllerSetupForPCSEvidenceNotes(bool enablePCSEvidenceNotes = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnablePCSEvidenceNotes = enablePCSEvidenceNotes;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForPCSEvidenceNotesAndPBSEvidenceNotes(bool enablePCSEvidenceNotes = false, bool enablePBSEvidenceNotes = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnablePCSEvidenceNotes = enablePCSEvidenceNotes;
            configService.CurrentConfiguration.EnablePBSEvidenceNotes = enablePBSEvidenceNotes;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForPBSEvidenceNotes(bool enablePBSEvidenceNotes = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnablePBSEvidenceNotes = enablePBSEvidenceNotes;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForAATFEvidenceNotes(bool enableAATFEvidenceNotes = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableAATFEvidenceNotes = enableAATFEvidenceNotes;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForAATFEvidenceNotesAndPBSEvidenceNotes(bool enableAATFEvidenceNotes = false, bool enablePBSEvidenceNotes = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableAATFEvidenceNotes = enableAATFEvidenceNotes;
            configService.CurrentConfiguration.EnablePBSEvidenceNotes = enablePBSEvidenceNotes;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeController(bool enableDataReturns = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableDataReturns = enableDataReturns;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForEnableDataReturnsAndEnablePBSEvidenceNotes(bool enableDataReturns = false, bool enablePBS = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableDataReturns = enableDataReturns;
            configService.CurrentConfiguration.EnablePBSEvidenceNotes = enablePBS;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForAATFReturns(bool enableAATFReturns = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableAATFReturns = enableAATFReturns;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForAATFReturnsAndPBSEvidenceNotes(bool enableAATFReturns = false, bool enablePBS = false)
        {
            var configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableAATFReturns = enableAATFReturns;
            configService.CurrentConfiguration.EnablePBSEvidenceNotes = enablePBS;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        /// <summary>
        /// This test ensures that a GET request to the "ManageContactDetails" action of the Home controller will
        /// fetch the organisation data and the list of countries, set the countries on the organsiation's address
        /// and then return the default view with the organisation data as the model.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetManageContactDetails_WithValidOrganisationId_GetsDataAndGetsCountriesAndReturnsDefaultView()
        {
            // Arrange
            var client = A.Fake<IWeeeClient>();

            var schemeData = new SchemeData() { Contact = new ContactData(), Address = new AddressData() };

            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(schemeData);

            var countries = new List<CountryData>();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            Func<IWeeeClient> apiClient = () => client;

            var cache = A.Dummy<IWeeeCache>();

            var breadcrumb = A.Dummy<BreadcrumbService>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();
            var configService = A.Dummy<ConfigurationService>();
            var controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);
            new HttpContextMocker().AttachToController(controller);

            // Act
            var id = new Guid("A4B50C6B-64FE-4119-ACDF-82C502B59BC8");
            var result = await controller.ManageContactDetails(id);

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemeByOrganisationId>.That.Matches(c => c.OrganisationId.Equals(id)))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._)).MustHaveHappened(1, Times.Exactly);
            Assert.Equal(countries, schemeData.Address.Countries);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);

            var viewResult = (ViewResult)result;

            Assert.Equal(string.Empty, viewResult.ViewName);
            Assert.Equal(schemeData, viewResult.Model);
        }

        /// <summary>
        /// This test ensures that a POST request to the "ManageContactDetails" action of the Home controller where
        /// the post data results in a model error will fetch the list of countries, set the countries on the
        /// organsiation's address and then return the default view with the organisation data as the model.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostManageContactDetails_WithModelErrors_GetsCountriesAndReturnsDefaultView()
        {
            // Arrange
            var schemeData = new SchemeData { Contact = new ContactData(), Address = new AddressData() };

            var client = A.Fake<IWeeeClient>();

            var countries = new List<CountryData>();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);
            Func<IWeeeClient> apiClient = () => client;

            var cache = A.Dummy<IWeeeCache>();
            var breadcrumb = A.Dummy<BreadcrumbService>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();
            var configService = A.Dummy<ConfigurationService>();

            var controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);
            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            // Act
            var result = await controller.ManageContactDetails(schemeData);

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.Equal(countries, schemeData.Address.Countries);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);

            var viewResult = (ViewResult)result;

            Assert.Equal(string.Empty, viewResult.ViewName);
            Assert.Equal(schemeData, viewResult.Model);
        }

        /// <summary>
        /// This test ensures that a POST request to the "ManageContactDetails" action of the Home controller where
        /// the post data results in no model errors will call the API to update the organisation details and
        /// then return a redirect result to the activity springboard page.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostManageContactDetails_WithNoModelErrors_UpdatesDetailsAndRedirectsToActivitySpringboard()
        {
            // Arrange
            var schemeData = new SchemeData { Contact = new ContactData(), Address = new AddressData() };

            var client = A.Fake<IWeeeClient>();

            A.CallTo(() => client.SendAsync(A<string>._, A<UpdateSchemeContactDetails>._)).Returns(true);

            Func<IWeeeClient> apiClient = () => client;

            var cache = A.Dummy<IWeeeCache>();
            var breadcrumb = A.Dummy<BreadcrumbService>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();
            var configService = A.Dummy<ConfigurationService>();
            var controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);
            new HttpContextMocker().AttachToController(controller);

            // Act
            var result = await controller.ManageContactDetails(schemeData);

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<UpdateSchemeContactDetails>._)).MustHaveHappened(1, Times.Exactly);

            Assert.NotNull(result);
            Assert.IsType(typeof(RedirectToRouteResult), result);

            var redirectResult = (RedirectToRouteResult)result;

            Assert.Equal("ChooseActivity", redirectResult.RouteValues["Action"]);
        }

        [Fact]
        public async Task PostManageContactDetails_UpdatesDetailsAndSendNotificationOnChange()
        {
            // Arrange
            var client = A.Fake<IWeeeClient>();

            Func<IWeeeClient> apiClient = () => client;

            var cache = A.Dummy<IWeeeCache>();

            var breadcrumb = A.Dummy<BreadcrumbService>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();
            var configService = A.Dummy<ConfigurationService>();
            var controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);

            // Act
            var result = await controller.ManageContactDetails(A.Dummy<SchemeData>());

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<UpdateSchemeContactDetails>._))
                .WhenArgumentsMatch(a => ((UpdateSchemeContactDetails)a[1]).SendNotificationOnChange == true)
                .MustHaveHappened();
        }

        [Fact]
        public async void GetViewSubmissionHistory_ShouldExecuteGetSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewSubmissionHistory_SchemeInfoIsNull_ShouldNotExecuteGetSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._)).Returns((SchemePublicInfo)null);

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .MustNotHaveHappened();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewSubmissionHistory_SortsByComplianceYearDescendingAsDefault()
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetSubmissionsHistoryResults>(1).Ordering == SubmissionsHistoryOrderBy.ComplianceYearDescending)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as SubmissionHistoryViewModel;

            Assert.Equal(SubmissionsHistoryOrderBy.ComplianceYearDescending, model.OrderBy);
        }

        [Fact]
        public async void GetViewSubmissionHistory_DoesNotRequestForSummaryData()
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetSubmissionsHistoryResults>(1).IncludeSummaryData == false)
                .MustHaveHappened();
        }

        [Theory]
        [InlineData(SubmissionsHistoryOrderBy.ComplianceYearAscending)]
        [InlineData(SubmissionsHistoryOrderBy.ComplianceYearDescending)]
        [InlineData(SubmissionsHistoryOrderBy.SubmissionDateAscending)]
        [InlineData(SubmissionsHistoryOrderBy.SubmissionDateDescending)]
        public async void GetViewSubmissionHistory_SortsBySpecifiedValue(SubmissionsHistoryOrderBy orderBy)
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>(), orderBy);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetSubmissionsHistoryResults>(1).Ordering == orderBy)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as SubmissionHistoryViewModel;

            Assert.Equal(orderBy, model.OrderBy);
        }

        [Fact]
        public async void GetDownloadCsv_ShouldExecuteGetMemberUploadDataAndReturnsCsvFile()
        {
            var controller = HomeController();

            var result = await controller.DownloadCsv(A.Dummy<Guid>(), A.Dummy<int>(), A.Dummy<Guid>(), A.Dummy<DateTime>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void GetViewDataReturnSubmissionHistory_ShouldExecuteGetDataReturnSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewDataReturnSubmissionHistory_SchemeInfoIsNull_ShouldNotExecuteGetDataReturnSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._)).Returns((SchemePublicInfo)null);

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .MustNotHaveHappened();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewDataReturnSubmissionHistory_SortsByComplianceYearDescendingAsDefault()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetDataReturnSubmissionsHistoryResults>(1).Ordering == DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as DataReturnSubmissionHistoryViewModel;

            Assert.Equal(DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending, model.OrderBy);
        }

        [Theory]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.QuarterAscending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.QuarterDescending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending)]
        public async void GetViewDataReturnSubmissionHistory_SortsBySpecifiedValue(DataReturnSubmissionsHistoryOrderBy orderBy)
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>(), orderBy);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetDataReturnSubmissionsHistoryResults>(1).Ordering == orderBy)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as DataReturnSubmissionHistoryViewModel;

            Assert.Equal(orderBy, model.OrderBy);
        }

        [Fact]
        public async Task GetViewDataReturnSubmissionHistory_DoesNotRequestForSummaryData()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).IncludeSummaryData == false)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetViewDataReturnSubmissionHistory_DoesNotRequestForEeeOutputDataComparison()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).CompareEeeOutputData == false)
                .MustHaveHappened();
        }

        [Fact]
        public async void PostChooseActivity_ManageEeeWeeeData_RedirectsToDataReturnsIndex()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Approved);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageEeeWeeeData
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("DataReturns", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_MakeAATFReturn_RedirectsToSelectYourPCS()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageAatfReturns
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
        }

        [Fact]
        public async void PostChooseActivity_ManagePBSEvidenceNotes_RedirectsHoldingIndex()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManagePBSEvidenceNotes
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("Holding", routeValues["controller"]);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasNoAatfs_ViewModelShouldNotContainManageAatfReturns()
        {
            var organisationData = new OrganisationData() { HasAatfs = false };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManageAatfReturns);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasAatfs_ViewModelShouldContainManageAatfReturns()
        {
            var organisationData = new OrganisationData() { HasAatfs = true };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManageAatfReturns);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasNoAes_ViewModelShouldNotContainManageAesReturns()
        {
            var organisationData = new OrganisationData() { HasAes = false };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManageAeReturns);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasAes_ViewModelShouldContainManageAeReturns()
        {
            var organisationData = new OrganisationData() { HasAes = true };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManageAeReturns);
        }

        [Fact]
        public async void ChooseActivityPOST_GivenOrganisationHasNoAatfs_ViewModelShouldNotContainManageAatfReturns()
        {
            var organisationData = new OrganisationData() { HasAatfs = false };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<ChooseActivityViewModel>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManageAatfReturns);
        }

        [Fact]
        public async void ChooseActivityPOST_GivenOrganisationHasAatfs_ViewModelShouldContainManageAatfReturns()
        {
            var organisationData = new OrganisationData() { HasAatfs = true };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<ChooseActivityViewModel>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManageAatfReturns);
        }

        [Fact]
        public async void ChooseActivityPOST_GivenOrganisationHasNoAes_ViewModelShouldNotContainManageAesReturns()
        {
            var organisationData = new OrganisationData() { HasAes = false };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<ChooseActivityViewModel>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManageAeReturns);
        }

        [Fact]
        public async void ChooseActivityPOST_GivenOrganisationHasAes_ViewModelShouldContainManageAeReturns()
        {
            var organisationData = new OrganisationData() { HasAes = true };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(true).ChooseActivity(A.Dummy<ChooseActivityViewModel>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManageAeReturns);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasNoScheme_ViewModelShouldNotContainManagePcsMembers()
        {
            var organisationData = new OrganisationData() { SchemeId = null };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeController(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePcsMembers);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasScheme_ViewModelShouldContainManagePcsMembers()
        {
            var organisationData = new OrganisationData() { SchemeId = Guid.NewGuid() };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeController(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManagePcsMembers);
        }

        [Fact]
        public async void ChooseActivityPOST_GivenOrganisationHasNoScheme_ViewModelShouldNotContainManagePcsMembers()
        {
            var organisationData = new OrganisationData() { SchemeId = null };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeController(true).ChooseActivity(A.Dummy<ChooseActivityViewModel>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePcsMembers);
        }

        [Fact]
        public async void ChooseActivityPOST_GivenOrganisationHasScheme_ViewModelShouldContainManagePcsMembers()
        {
            var organisationData = new OrganisationData() { SchemeId = Guid.NewGuid() };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeController(true).ChooseActivity(A.Dummy<ChooseActivityViewModel>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManagePcsMembers);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasAesAndAatfsButAatfReturnsIsOff_ViewModelShouldNotContainManageReturns()
        {
            var organisationData = new OrganisationData() { HasAes = true, HasAatfs = true };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForAATFReturns(false).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManageAeReturns);
            model.PossibleValues.Should().NotContain(PcsAction.ManageAatfReturns);
        }

        [Fact]
        public async Task GetActivities_WithEnablePCSEvidenceNotesConfigurationSetToTrueAndOrganisationHasAnAatf_ReturnsPCSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = A.Dummy<Guid>()
              });

            var result = await HomeControllerSetupForPCSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            result.Should().Contain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnablePCSEvidenceNotesConfigurationSetToTrueAndOrganisationHasNoSchemeId_DoesNotReturnPCSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = null
              });

            var result = await HomeControllerSetupForPCSEvidenceNotes(true).GetActivities(A.Dummy<Guid>());

            result.Should().NotContain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasNoScheme_ViewModelShouldNotContainManagePcsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { SchemeId = null };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeController(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasScheme_ManagePcsEvidenceNotesSetToTrueInconfig_ViewModelShouldContainManagePcsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { SchemeId = Guid.NewGuid() };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForPCSEvidenceNotes(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenOrganisationHasScheme_ManagePcsEvidenceNotesSetToFalseInconfig_ViewModelShouldNotContainManagePcsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { SchemeId = Guid.NewGuid() };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForPCSEvidenceNotes(false).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnablePCSEvidenceNotesConfigurationSetToTrueAndOrganisationHasAnAatfAndEnablePBSEvidenceNotesIsSetToFalse_ReturnsPCSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = A.Dummy<Guid>(),
              });

            var result = await HomeControllerSetupForPCSEvidenceNotesAndPBSEvidenceNotes(true, false).GetActivities(A.Dummy<Guid>());

            result.Should().Contain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async Task GetActivities_WithEnablePCSEvidenceNotesConfigurationSetToTrueAndOrganisationHasAnAatfAndEnabledPBSIsSetToTrue_ShouldNotReturnsPCSEvidenceNotesOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
              .Returns(new OrganisationData
              {
                  SchemeId = A.Dummy<Guid>()
              });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData
                {
                    IsBalancingScheme = true
                });

            var result = await HomeControllerSetupForPCSEvidenceNotesAndPBSEvidenceNotes(true, true).GetActivities(A.Dummy<Guid>());

            result.Should().NotContain(PcsAction.ManagePcsEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenIsBalancingSchemeIsSetToFalse_ViewModelShouldNotContainManagePbsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { SchemeId = null };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeController(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePBSEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenIsBalancingSchemeIsSetToTrue_ManagePbsEvidenceNotesSetToTrueInconfig_ViewModelShouldContainManagePbsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { IsBalancingScheme = true };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().Contain(PcsAction.ManagePBSEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenIsBalancingSchemeIsSetToFalse_ManagePbsEvidenceNotesSetToTrueInconfig_ViewModelShouldNotContainManagePbsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { IsBalancingScheme = false };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForPBSEvidenceNotes(true).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePBSEvidenceNotes);
        }

        [Fact]
        public async void ChooseActivityGET_GivenIsBalancingSchemeIsSetToFalse_ManagePbsEvidenceNotesSetToFalseInconfig_ViewModelShouldNotContainManagePbsEvidenceNotes()
        {
            var organisationData = new OrganisationData() { IsBalancingScheme = false };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._)).Returns(organisationData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._)).Returns(true);

            var result = await HomeControllerSetupForPBSEvidenceNotes(false).ChooseActivity(A.Dummy<Guid>()) as ViewResult;

            var model = result.Model as ChooseActivityViewModel;
            model.PossibleValues.Should().NotContain(PcsAction.ManagePBSEvidenceNotes);
        }
    }
}
