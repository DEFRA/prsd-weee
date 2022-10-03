﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using AutoFixture;
    using Core.Scheme;
    using Core.Shared;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ViewTransferNoteViewModelMapTests : SimpleUnitTestBase
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly ViewTransferNoteViewModelMap map;

        public ViewTransferNoteViewModelMapTests()
        {
            addressUtilities = A.Fake<IAddressUtilities>();
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            map = new ViewTransferNoteViewModelMap(addressUtilities, tonnageUtilities);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSource_ViewTransferNoteViewModelShouldBeReturned()
        {
            //arrange
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var user = A.Fake<IPrincipal>();

            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, user);

            //act
            var model = map.Map(source);

            //assert
            model.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]

        public void ViewTransferNoteViewModelMap_GivenSourceWithReturnToViewAsFalse_PropertiesShouldBeSet(bool returnToView)
        {
            //arrange
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var principal = A.Fake<IPrincipal>();

            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, principal);
            source.ReturnToView = returnToView;

            //act
            var model = map.Map(source);

            //assert
            model.ReturnToView.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]

        public void ViewTransferNoteViewModelMap_GivenSourceWithOpenedInNewTab_PropertiesShouldBeSet(bool openedInNewTab)
        {
            //arrange
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var principal = A.Fake<IPrincipal>();

            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, principal)
                {
                    OpenedInNewTab = openedInNewTab
                };

            //act
            var model = map.Map(source);

            //assert
            model.OpenedInNewTab.Should().Be(openedInNewTab);
        }

        [Fact]

        public void ViewTransferNoteViewModelMap_GivenSourceWithPage_PagePropertyShouldBeSet()
        {
            //arrange
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var principal = A.Fake<IPrincipal>();
            var page = TestFixture.Create<int>();

            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, principal)
            {
                Page = page
            };

            //act
            var model = map.Map(source);

            //assert
            model.Page.Should().Be(page);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSourceWithReturnToViewAsTrue_ShouldSetReturnToView()
        {
            //arrange
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var user = A.Fake<IPrincipal>();
            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, user);
            
            source.ReturnToView = true;

            //act
            var model = map.Map(source);

            //assert
            model.ReturnToView.Should().BeTrue();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSourceWithRedirectTab_ShouldSetRedirectTab()
        {
            //arrange
            var tab = TestFixture.Create<string>();
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var user = A.Fake<IPrincipal>();
            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, user);

            source.RedirectTab = tab;

            //act
            var model = map.Map(source);

            //assert
            model.RedirectTab.Should().Be(tab);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ViewTransferNoteViewModelMap_GivenSource_PropertiesShouldBeSet(bool editMode)
        {
            //arrange
            var user = A.Fake<IPrincipal>();
            var orgId = TestFixture.Create<Guid>();
            var transferEvidenceNoteData = TestFixture.Create<TransferEvidenceNoteData>();
            var displayNotification = TestFixture.Create<object>();
            var source = new ViewTransferNoteViewModelMapTransfer(orgId, transferEvidenceNoteData, displayNotification, user);

            source.Edit = editMode;

            //act
            var model = map.Map(source);

            //assert
            model.SchemeId.Should().Be(source.OrganisationId);
            model.EvidenceNoteId.Should().Be(source.TransferEvidenceNoteData.Id);
            model.EditMode.Should().Be(editMode);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSourceNoteType_PropertiesShouldBeSet()
        {
            //arrange
            var justNow = SystemTime.UtcNow;
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>().With(t1 => t1.Type, NoteType.Transfer).Create(),
                    null);
            source.TransferEvidenceNoteData.VoidedDate = justNow;
            source.TransferEvidenceNoteData.VoidedReason = "some wonderful reason";

            //act
            var model = map.Map(source);

            //assert
            model.Type.Should().Be(source.TransferEvidenceNoteData.Type);
            model.VoidedReason.Should().Be(source.TransferEvidenceNoteData.VoidedReason);
            model.VoidedDate.Should().Be(source.TransferEvidenceNoteData.VoidedDate.ToDisplayGMTDateTimeString());
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenSourceNoteStatus_PropertiesShouldBeSet(NoteStatus status)
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>().With(t1 => t1.Status, status).Create(),
                null);

            //act
            var model = map.Map(source);

            //assert
            model.Status.Should().Be(source.TransferEvidenceNoteData.Status);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsDraft_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Draft);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully saved the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference} as a draft");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_WithInternalAdmin_CanVoidShouldBeTrue()
        {
            //arrange
            var user = A.Fake<IPrincipal>();
            var principal = new ClaimsPrincipal(user);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(user.Identity);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Claims.InternalAdmin));
            principal.AddIdentity(claimsIdentity);

            A.CallTo(() => user.Identity).Returns(claimsIdentity);

            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Submitted, user);

            //act
            var model = map.Map(source);

            //assert
            model.CanVoid.Should().BeTrue();
         }

        [Fact]
        public void ViewTransferNoteViewModelMap_WithExternalUser_CanVoidShouldBeFalse()
        {
            //arrange
            var user = A.Fake<IPrincipal>();
            var principal = new ClaimsPrincipal(user);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(user.Identity);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Claims.CanAccessExternalArea));
            principal.AddIdentity(claimsIdentity);

            A.CallTo(() => user.Identity).Returns(claimsIdentity);

            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Submitted, user);

            //act
            var model = map.Map(source);

            //assert
            model.CanVoid.Should().BeFalse();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_WithNullUser_CanVoidShouldBeFalse()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Submitted, null);

            //act
            var model = map.Map(source);

            //assert
            model.CanVoid.Should().BeFalse();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsSubmitted_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Submitted);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully submitted the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsReturnedSaved_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.ReturnedSaved);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully saved the returned evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsApproved_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Approved);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should().Be($"You have approved the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsRejected_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Rejected);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should().Be($"You have rejected the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsReturned_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Returned);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should().Be($"You have returned the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsReturnedSubmitted_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.ReturnedSubmitted);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully submitted the returned evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsVoided_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                NoteUpdatedStatusEnum.Void);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should().Be($"You have successfully voided the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Theory]
        [InlineData(null, NoteStatus.Draft)]
        [InlineData(false, NoteStatus.Draft)]
        [InlineData(null, NoteStatus.Submitted)]
        [InlineData(false, NoteStatus.Submitted)]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationIsFalse_SuccessMessageShouldBeEmpty(bool? display, NoteStatus status)
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(t2 => t2.Status, status)
                    .With(t1 => t1.Reference, 1).Create(),
                display);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenTransfer_CallsAddressUtilities()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.ApprovedTransfererDetails, string.Empty)
                    .With(x => x.ApprovedRecipientDetails, string.Empty)
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData()).Create(),
                false);

            //act
            map.Map(source);

            //assert
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenTransfer_PropertiesShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageData()).Create(),
                false)
            {
                ReturnToView = true
            };

            //act
            var model = map.Map(source);

            //assert
            model.ComplianceYear.Should().Be(source.TransferEvidenceNoteData.ComplianceYear);
            model.ReturnToView.Should().Be(source.ReturnToView.Value);
            model.EditMode.Should().Be(source.Edit);
            model.Reference.Should().Be(source.TransferEvidenceNoteData.Reference);
            model.Type.Should().Be(source.TransferEvidenceNoteData.Type);
            model.Status.Should().Be(source.TransferEvidenceNoteData.Status);
            model.SchemeId.Should().Be(source.OrganisationId);
            model.EvidenceNoteId.Should().Be(source.TransferEvidenceNoteData.Id);
            model.TotalCategoryValues.Count.Should().Be(3);
            model.Summary.Count.Should().Be(2);
            model.Summary[0].Notes.Count.Should().Be(2);
            model.Summary[1].Notes.Count.Should().Be(1);
        }

        [Fact]
        public void Map_GivenSource_RecipientAddressShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.ApprovedTransfererDetails, string.Empty)
                    .With(x => x.ApprovedRecipientDetails, string.Empty)
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData()).Create(),
                false);
            const string siteAddress = "siteAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientSchemeData.SchemeName),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientOrganisationData.OrganisationName),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.Address1),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.Address2),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.TownOrCity),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.CountyOrRegion),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.Postcode),
                null)).Returns(siteAddress);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientAddress.Should().Be(siteAddress);
        }

        [Fact]
        public void Map_GivenSource_TransferredByAddressIsNotBalancingSchemeAddressShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.ApprovedTransfererDetails, string.Empty)
                    .With(x => x.ApprovedRecipientDetails, string.Empty)
                    .Create(),
                false);
            const string siteAddress = "siteAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredSchemeData.SchemeName),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredOrganisationData.OrganisationName),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.Address1),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.Address2),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.TownOrCity),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.CountyOrRegion),
                A<string>.That.Matches(x => x == source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.Postcode),
                null)).Returns(siteAddress);

            //act
            var result = map.Map(source);

            //assert
            result.TransferredByAddress.Should().Be(siteAddress);
        }

        [Fact]
        public void Map_GivenSource_TransferredByAddressIsBalancingSchemeAddressShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData(true))
                    .Create(),
                false);
            
            //act
            var result = map.Map(source);

            //assert
            result.TransferredByAddress.Should().Be(source.TransferEvidenceNoteData.TransferredOrganisationData.OrganisationName);
        }

        [Fact]
        public void Map_GivenSource_TransferredByAddress_SchemeAddressShouldOnlyBeMappedOnce()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.ApprovedTransfererDetails, string.Empty)
                    .With(x => x.ApprovedRecipientDetails, string.Empty)
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData(true))
                    .Create(),
                false);
           
            //act
            var result = map.Map(source);

            //assert
            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(A<string>._, A<string>._, A<string>._,
                A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenTransfer_CorrectlySumsAndOrdersTotalCategories()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageData()).Create(),
                false);

            //act
            var model = map.Map(source);

            //assert
            model.TotalCategoryValues.Should().BeInAscendingOrder(x => x.CategoryId);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenTransfer_CorrectlyGeneratesViewTransferEvidenceAatfDataViewModel_InCorrectOrder()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageData()).Create(),
                false);

            //act
            var model = map.Map(source);

            //assert
            model.Summary.Should().BeInAscendingOrder(x => x.AatfName);
            model.Summary[0].Notes.Should().BeInDescendingOrder(x => x.ReferenceId);
            model.Summary[1].Notes.Should().BeInDescendingOrder(x => x.ReferenceId);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenTransfer_DisplayCategoryDoesNotDisplayZeroedCategories()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageDataWithNoTonnage()).Create(),
                false);

            //act
            var model = map.Map(source);

            //assert
            model.TotalCategoryValues.Should().OnlyContain(x => x.DisplaySummedCategory == false);
        }
        
        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSubmittedTransfer_HasSubmittedDate_WithCorrectFormat()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageData()).Create(),
                false)
            {
                TransferEvidenceNoteData =
                {
                    SubmittedDate = DateTime.Parse("01/01/2001 13:30:30")
                }
            };

            //act
            var model = map.Map(source);

            //assert
            model.HasSubmittedDate.Should().BeTrue();
            model.SubmittedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsNotDraftOrReturned_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (status == NoteStatus.Draft || status == NoteStatus.Returned)
            {
                return;
            }

            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status).Create(),
                false);

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsDraftOrReturnedAndOrganisationIsTransferOrganisationAndSchemeIsValid_DisplayEditButtonShouldBeTrue(NoteStatus status)
        {
            if (status != NoteStatus.Draft && status != NoteStatus.Returned)
            {
                return;
            }

            var transferOrganisationId = TestFixture.Create<Guid>();
            var date = new DateTime(2022, 1, 1);
            var source = new ViewTransferNoteViewModelMapTransfer(transferOrganisationId,
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status)
                    .With(x => x.ComplianceYear, date.Year)
                    .With(x => x.TransferredSchemeData, TestFixture.Build<SchemeData>().With(s => s.SchemeStatus, SchemeStatus.Approved).Create())
                    .With(x => x.TransferredOrganisationData, 
                        TestFixture.Build<OrganisationData>()
                            .With(o => o.Id, transferOrganisationId)
                            .With(o => o.IsBalancingScheme, false)
                            .Create())
                    .Create(),
                false)
            {
                SystemDateTime = date
            };

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsDraftOrReturnedButOrganisationIsNotTransferOrganisationAndSchemeIsValid_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (status != NoteStatus.Draft && status != NoteStatus.Returned)
            {
                return;
            }

            var transferOrganisationId = TestFixture.Create<Guid>();
            var date = new DateTime(2022, 1, 1);
            var source = new ViewTransferNoteViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status)
                    .With(x => x.ComplianceYear, date.Year)
                    .With(x => x.TransferredSchemeData, TestFixture.Build<SchemeData>().With(s => s.SchemeStatus, SchemeStatus.Approved).Create())
                    .With(x => x.TransferredOrganisationData,
                        TestFixture.Build<OrganisationData>()
                            .With(o => o.Id, transferOrganisationId)
                            .With(o => o.IsBalancingScheme, false)
                            .Create())
                    .Create(),
                false)
            {
                SystemDateTime = date
            };

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsDraftOrReturnedOrganisationIsTransferOrganisationButSchemeIsWithdrawn_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (status != NoteStatus.Draft && status != NoteStatus.Returned)
            {
                return;
            }

            var transferOrganisationId = TestFixture.Create<Guid>();
            var date = new DateTime(2022, 1, 1);
            var source = new ViewTransferNoteViewModelMapTransfer(transferOrganisationId,
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status)
                    .With(x => x.ComplianceYear, date.Year)
                    .With(x => x.TransferredSchemeData, TestFixture.Build<SchemeData>().With(s => s.SchemeStatus, SchemeStatus.Withdrawn).Create())
                    .With(x => x.TransferredOrganisationData,
                        TestFixture.Build<OrganisationData>()
                            .With(o => o.Id, transferOrganisationId)
                            .With(o => o.IsBalancingScheme, false)
                            .Create())
                    .Create(),
                false)
            {
                SystemDateTime = date
            };

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsDraftOrReturnedOrganisationIsTransferOrganisationButSchemeOutOfComplianceYear_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (status != NoteStatus.Draft && status != NoteStatus.Returned)
            {
                return;
            }

            var transferOrganisationId = TestFixture.Create<Guid>();
            var date = new DateTime(2023, 2, 1);
            var source = new ViewTransferNoteViewModelMapTransfer(transferOrganisationId,
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status)
                    .With(x => x.ComplianceYear, date.Year - 1)
                    .With(x => x.TransferredSchemeData, TestFixture.Build<SchemeData>().With(s => s.SchemeStatus, SchemeStatus.Approved).Create())
                    .With(x => x.TransferredOrganisationData,
                        TestFixture.Build<OrganisationData>()
                            .With(o => o.Id, transferOrganisationId)
                            .With(o => o.IsBalancingScheme, false)
                            .Create())
                    .Create(),
                false)
            {
                SystemDateTime = date
            };

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsDraftOrReturnedOrganisationIsTransferOrganisationAndOrganisationIsBalancingSchemeOutOfComplianceYear_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (status != NoteStatus.Draft && status != NoteStatus.Returned)
            {
                return;
            }

            var transferOrganisationId = TestFixture.Create<Guid>();
            var date = new DateTime(2023, 2, 1);
            var source = new ViewTransferNoteViewModelMapTransfer(transferOrganisationId,
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status)
                    .With(x => x.ComplianceYear, date.Year - 1)
                    .With(x => x.TransferredSchemeData, TestFixture.Build<SchemeData>().With(s => s.SchemeStatus, SchemeStatus.Approved).Create())
                    .With(x => x.TransferredOrganisationData,
                        TestFixture.Build<OrganisationData>()
                            .With(o => o.Id, transferOrganisationId)
                            .With(o => o.IsBalancingScheme, true)
                            .Create())
                    .Create(),
                false)
            {
                SystemDateTime = date
            };

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenNoteStatusIsDraftOrReturnedOrganisationIsTransferOrganisationButOrganisationIsBalancingScheme_DisplayEditButtonShouldBeTrue(NoteStatus status)
        {
            if (status != NoteStatus.Draft && status != NoteStatus.Returned)
            {
                return;
            }

            var transferOrganisationId = TestFixture.Create<Guid>();
            var date = new DateTime(2022, 2, 1);
            var source = new ViewTransferNoteViewModelMapTransfer(transferOrganisationId,
                TestFixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.Status, status)
                    .With(x => x.ComplianceYear, date.Year)
                    .With(x => x.TransferredSchemeData, TestFixture.Build<SchemeData>().With(s => s.SchemeStatus, SchemeStatus.Withdrawn).Create())
                    .With(x => x.TransferredOrganisationData,
                        TestFixture.Build<OrganisationData>()
                            .With(o => o.Id, transferOrganisationId)
                            .With(o => o.IsBalancingScheme, true)
                            .Create())
                    .Create(),
                false)
            {
                SystemDateTime = date
            };

            //act
            var model = map.Map(source);

            //assert
            model.DisplayEditButton.Should().BeTrue();
        }

        private OrganisationData CreateOrganisationData(bool balancingScheme = false)
        {
            return new OrganisationData()
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                OrganisationName = "Name",
                TradingName = "Trading Name",
                CompanyRegistrationNumber = "123456",
                HasBusinessAddress = true,
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Address1",
                    Address2 = "Address2",
                    CountryName = "France",
                    CountyOrRegion = "County",
                    TownOrCity = "Town",
                    Postcode = "GU22 7UY",
                    Telephone = "987654",
                    Email = "test@test.com"
                },
                IsBalancingScheme = balancingScheme
            };
        }
        private IList<TransferEvidenceNoteTonnageData> CreateTransferEvidenceNoteTonnageData()
        {
            //Aatf1
            var aatf1 = new AatfData()
            {
                Name = "Test One",
                ApprovalNumber = "Approval",
            };

            //Aatf2
            var aatf2 = new AatfData()
            {
                Name = "Test Two",
                ApprovalNumber = "ApprovalTwo",
            };

            //Note Ids
            var note1Id = Guid.NewGuid();
            var note2Id = Guid.NewGuid();
            var note3Id = Guid.NewGuid();

            // Note 1 - Cat1
            var noteCat1 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note1Id,
                OriginalReference = 1000,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.LargeHouseholdAppliances, 20, 20, 10, 10)
            };

            // Note 1 - Cat2
            var noteCat2 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note1Id,
                OriginalReference = 1000,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.SmallHouseholdAppliances, 20, 20, 10, 10)
            };

            // Note 1 - Cat3
            var noteCat3 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note1Id,
                OriginalReference = 1000,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, 20, 20, 10, 10)
            };

            // Note 2 - Cat1
            var note2Cat1 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note2Id,
                OriginalReference = 1001,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.LargeHouseholdAppliances, 20, 20, 10, 10)
            };

            // Note 2 - Cat2
            var note2Cat2 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note2Id,
                OriginalReference = 1001,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.SmallHouseholdAppliances, 20, 20, 10, 10)
            };

            // Note 2 - Cat3
            var note2Cat3 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note2Id,
                OriginalReference = 1001,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, 20, 20, 10, 10)
            };

            // Note 3 - Cat1
            var note3Cat1 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.LargeHouseholdAppliances, 20, 20, 10, 10)
            };

            // Note 3 - Cat2
            var note3Cat2 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.SmallHouseholdAppliances, 20, 20, 10, 10)
            };

            // Note 3 - Cat3
            var note3Cat3 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, 20, 20, 10, 10)
            };

            // Note 3 - Cat4
            var note3Cat4 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, null, null, null, null)
            };

            return new List<TransferEvidenceNoteTonnageData>() { noteCat1, noteCat2, noteCat3, note2Cat1, note2Cat2, note2Cat3, note3Cat1, note3Cat2, note3Cat3, note3Cat4 };
        }

        private IList<TransferEvidenceNoteTonnageData> CreateTransferEvidenceNoteTonnageDataWithNoTonnage()
        {
            //Aatf1
            var aatf1 = new AatfData()
            {
                Name = "Test One",
                ApprovalNumber = "Approval",
            };

            //Aatf2
            var aatf2 = new AatfData()
            {
                Name = "Test Two",
                ApprovalNumber = "ApprovalTwo",
            };

            //Note Ids
            var note1Id = Guid.NewGuid();
            var note2Id = Guid.NewGuid();
            var note3Id = Guid.NewGuid();

            // Note 1 - Cat1
            var noteCat1 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note1Id,
                OriginalReference = 1000,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.LargeHouseholdAppliances, 0, 0, 0, 0)
            };

            // Note 1 - Cat2
            var noteCat2 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note1Id,
                OriginalReference = 1000,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.SmallHouseholdAppliances, 0, 0, 0, 0)
            };

            // Note 1 - Cat3
            var noteCat3 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note1Id,
                OriginalReference = 1000,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, 0, 0, 0, 0)
            };

            // Note 2 - Cat1
            var note2Cat1 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note2Id,
                OriginalReference = 1001,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.LargeHouseholdAppliances, 0, 0, 0, 0)
            };

            // Note 2 - Cat2
            var note2Cat2 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note2Id,
                OriginalReference = 1001,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.SmallHouseholdAppliances, 0, 0, 0, 0)
            };

            // Note 2 - Cat3
            var note2Cat3 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf1,
                OriginalNoteId = note2Id,
                OriginalReference = 1001,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, 0, 0, 0, 0)
            };

            // Note 3 - Cat1
            var note3Cat1 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.LargeHouseholdAppliances, 0, 0, 0, 0)
            };

            // Note 3 - Cat2
            var note3Cat2 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.SmallHouseholdAppliances, 0, 0, 0, 0)
            };

            // Note 3 - Cat3
            var note3Cat3 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, 0, 0, 0, 0)
            };

            // Note 3 - Cat4
            var note3Cat4 = new TransferEvidenceNoteTonnageData()
            {
                OriginalAatf = aatf2,
                OriginalNoteId = note3Id,
                OriginalReference = 1002,
                Type = NoteType.Transfer,
                EvidenceTonnageData = new EvidenceTonnageData(Guid.NewGuid(), Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment, null, null, null, null)
            };

            return new List<TransferEvidenceNoteTonnageData>() { noteCat1, noteCat2, noteCat3, note2Cat1, note2Cat2, note2Cat3, note3Cat1, note3Cat2, note3Cat3, note3Cat4 };
        }
    }
}
