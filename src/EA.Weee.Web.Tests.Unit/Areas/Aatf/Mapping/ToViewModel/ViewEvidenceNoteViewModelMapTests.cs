﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using FakeItEasy;
    using FluentAssertions;
    using Security;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ViewEvidenceNoteViewModelMapTests : SimpleUnitTestBase
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;
        private readonly IMapper mapper;
        private readonly ViewEvidenceNoteViewModelMap map;

        public ViewEvidenceNoteViewModelMapTests()
        {
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            addressUtilities = A.Fake<IAddressUtilities>();
            mapper = A.Fake<IMapper>();

            map = new ViewEvidenceNoteViewModelMap(tonnageUtilities, addressUtilities, mapper);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var result = Record.Exception(() => map.Map(null));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_GivenSource_StandardPropertiesShouldBeMapped(bool printable)
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, printable);

            //act
            var result = map.Map(source);

            //assert
            result.Id.Should().Be(source.EvidenceNoteData.Id);
            result.OrganisationId.Should().Be(source.EvidenceNoteData.OrganisationData.Id);
            result.AatfId.Should().Be(source.EvidenceNoteData.AatfData.Id);
            result.Reference.Should().Be(source.EvidenceNoteData.Reference);
            result.Status.Should().Be(source.EvidenceNoteData.Status);
            result.Type.Should().Be(source.EvidenceNoteData.Type);
            result.StartDate.Should().Be(source.EvidenceNoteData.StartDate);
            result.EndDate.Should().Be(source.EvidenceNoteData.EndDate);
            result.ProtocolValue.Should().Be(source.EvidenceNoteData.Protocol);
            result.WasteTypeValue.Should().Be(source.EvidenceNoteData.WasteType);
            result.SchemeId.Should().Be(source.SchemeId);
            result.SubmittedBy.Should().Be(source.EvidenceNoteData.AatfData.Name);
            result.AatfApprovalNumber.Should().Be(source.EvidenceNoteData.AatfData.ApprovalNumber);
            result.ComplianceYear.Should().Be(source.EvidenceNoteData.ComplianceYear);
            result.IsPrintable.Should().Be(printable);
        }

        [Fact]
        public void Map_GivenSource_WithBalancingScheme_RecipientAddressShouldBeMapped()
        {
            //arrange
            var evidenceNoteData = TestFixture.Create<EvidenceNoteData>();
            evidenceNoteData.RecipientOrganisationData.IsBalancingScheme = true;
            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, NoteUpdatedStatusEnum.Draft, true);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientAddress.Should().Be(source.EvidenceNoteData.RecipientOrganisationData.OrganisationName);
        }

        [Fact]
        public void Map_GivenSource_OperatorAddressShouldBeSet()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());
            const string operatorAddress = "operatorAddress";

            A.CallTo(() => addressUtilities.FormattedAddress(source.EvidenceNoteData.OrganisationData.OrganisationName,
                source.EvidenceNoteData.OrganisationData.BusinessAddress.Address1,
                source.EvidenceNoteData.OrganisationData.BusinessAddress.Address2,
                source.EvidenceNoteData.OrganisationData.BusinessAddress.TownOrCity,
                source.EvidenceNoteData.OrganisationData.BusinessAddress.CountyOrRegion,
                source.EvidenceNoteData.OrganisationData.BusinessAddress.Postcode,
                null)).Returns(operatorAddress);

            //act
            var result = map.Map(source);

            //assert
            result.OperatorAddress.Should().Be(operatorAddress);
        }

        [Fact]
        public void Map_GivenSource_SiteAddressShouldBeSet()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());
            const string siteAddress = "siteAddress";

            A.CallTo(() => addressUtilities.FormattedAddress(source.EvidenceNoteData.AatfData.SiteAddress.Name,
                source.EvidenceNoteData.AatfData.SiteAddress.Address1,
                source.EvidenceNoteData.AatfData.SiteAddress.Address2,
                source.EvidenceNoteData.AatfData.SiteAddress.TownOrCity,
                source.EvidenceNoteData.AatfData.SiteAddress.CountyOrRegion,
                source.EvidenceNoteData.AatfData.SiteAddress.Postcode,
                source.EvidenceNoteData.AatfData.ApprovalNumber)).Returns(siteAddress);

            //act
            var result = map.Map(source);

            //assert
            result.SiteAddress.Should().Be(siteAddress);
        }

        [Fact]
        public void Map_GivenSourceWithRecipientThatHasBusinessAddress_RecipientAddressShouldBeSetToApprovedRecipientDetails()
        {
            //arrange
            var organisation = TestFixture.Build<OrganisationData>()
                .With(o => o.HasBusinessAddress, true)
                .With(o => o.OrganisationName, "org").Create();
            var evidenceData = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.RecipientOrganisationData, organisation)
                .Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceData, null, TestFixture.Create<bool>());
            
            const string recipientAddress = "recipientAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.RecipientSchemeData.SchemeName,
                organisation.OrganisationName,
                organisation.BusinessAddress.Address1,
                organisation.BusinessAddress.Address2,
                organisation.BusinessAddress.TownOrCity,
                organisation.BusinessAddress.CountyOrRegion,
                organisation.BusinessAddress.Postcode,
                null)).Returns(recipientAddress);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientAddress.Should().Be(evidenceData.ApprovedRecipientDetails);
        }

        [Fact]
        public void Map_GivenSourceWithRecipientThatDoesNotHaveBusinessAddress_RecipientAddressShouldBeSetToNotificationAddress()
        {
            //arrange
            var organisation = TestFixture.Build<OrganisationData>()
                .With(o => o.HasBusinessAddress, false)
                .With(o => o.OrganisationName, "org").Create();
            var evidenceData = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.RecipientOrganisationData, organisation)
                .With(f => f.ApprovedRecipientDetails, string.Empty)
                .Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceData, null, TestFixture.Create<bool>());

            const string recipientAddress = "recipientAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.RecipientSchemeData.SchemeName,
                organisation.OrganisationName,
                organisation.NotificationAddress.Address1,
                organisation.NotificationAddress.Address2,
                organisation.NotificationAddress.TownOrCity,
                organisation.NotificationAddress.CountyOrRegion,
                organisation.NotificationAddress.Postcode,
                null)).Returns(recipientAddress);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientAddress.Should().Be(recipientAddress);
        }

        [Fact]
        public void Map_GivenSourceWithRecipientThatDoesNotHaveApprovedRecipientDetails_RecipientAddressShouldBeSetToNotificationAddress()
        {
            //arrange
            var organisation = TestFixture.Build<OrganisationData>()
                .With(o => o.HasBusinessAddress, false)
                .With(o => o.OrganisationName, "org").Create();
            var evidenceData = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.RecipientOrganisationData, organisation)
                .With(f => f.ApprovedRecipientDetails, "approved recipient details")
                .Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceData, null, TestFixture.Create<bool>());

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.RecipientSchemeData.SchemeName,
                organisation.OrganisationName,
                organisation.NotificationAddress.Address1,
                organisation.NotificationAddress.Address2,
                organisation.NotificationAddress.TownOrCity,
                organisation.NotificationAddress.CountyOrRegion,
                organisation.NotificationAddress.Postcode,
                null)).Returns("recipientAddress");

            //act
            var result = map.Map(source);

            //assert
            result.RecipientAddress.Should().Be("approved recipient details");
        }

        [Fact]
        public void Map_GivenTonnagesAndIncludeAllCategories_TonnagesShouldBeFormatted()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>())
                {
                    IncludeAllCategories = true,
                    EvidenceNoteData =
                    {
                        EvidenceTonnageData = new List<EvidenceTonnageData>()
                        {
                            new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ConsumerEquipment, null, 1, null, null),
                            new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ElectricalAndElectronicTools, 2, null, null, null)
                        }
                    }
                };

            A.CallTo(() =>
                    tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Received))
                .Returns(null);
            A.CallTo(() =>
                    tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Reused))
                .Returns("1");
            A.CallTo(() =>
                    tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(1).Received))
                .Returns("2");
            A.CallTo(() =>
                    tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(1).Reused))
                .Returns(null);

            //act
            var result = map.Map(source);

            //assert
            result.CategoryValues.Count.Should().Be(14);

            var consumerCategory =
                result.CategoryValues.First(c => c.CategoryId.Equals(WeeeCategory.ConsumerEquipment.ToInt()));
            consumerCategory.Received.Should().Be(null);
            consumerCategory.Reused.Should().Be("1");
            consumerCategory.Id.Should().Be(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Id);

            var electricalCategory = result.CategoryValues.First(c =>
                c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools.ToInt()));
            electricalCategory.Received.Should().Be("2");
            electricalCategory.Reused.Should().Be(null);
            electricalCategory.Id.Should().Be(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(1).Id);

            foreach (var evidenceCategoryValue in result.CategoryValues.Where(c => !c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools.ToInt())
                     && !c.CategoryId.Equals(WeeeCategory.ConsumerEquipment.ToInt())))
            {
                evidenceCategoryValue.Reused.Should().BeNull();
                evidenceCategoryValue.Received.Should().BeNull();
            }
        }

        [Fact]
        public void Map_GivenTonnagesAndNotIncludeAllCategories_OnlyCategoriesThatNoteTonnageShouldBeIncluded()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());
            source.IncludeAllCategories = false;

            source.EvidenceNoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ElectricalAndElectronicTools, 2, null, null, null)
            };

            A.CallTo(() =>
                    tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Received))
                .Returns("2");
            A.CallTo(() =>
                    tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Reused))
                .Returns(null);

            //act
            var result = map.Map(source);

            //assert
            result.CategoryValues.Count.Should().Be(1);

            var electricalCategory = result.CategoryValues.First(c => c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools.ToInt()));
            electricalCategory.Received.Should().Be("2");
            electricalCategory.Reused.Should().Be(null);
            electricalCategory.Id.Should().Be(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Id);
        }

        [Fact]
        public void Map_GivenTonnages_TotalReceivedShouldBeSet()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());

            source.EvidenceNoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.ConsumerEquipment, 1, 5, null, null),
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.ElectricalAndElectronicTools, 2, null, null, null),
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.GasDischargeLampsAndLedLightSources, 3, 20, null, null),
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.ITAndTelecommsEquipment, null, 50, null, null)
            };

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(0).Received)).Returns("1");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(1).Received)).Returns("2");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(2).Received)).Returns("3");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(source.EvidenceNoteData.EvidenceTonnageData.ElementAt(3).Received)).Returns("-");

            //act
            var result = map.Map(source);

            //assert
            result.TotalReceivedDisplay.Should().Be("6.000");
        }

        [Fact]
        public void Map_GivenNoteStatusIsNull_SuccessMessageShouldNotBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), null, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.DisplayMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenNoteStatusDraftCreated_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be($"You have successfully saved the evidence note with reference ID E{source.EvidenceNoteData.Reference} as a draft");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNoteStatusSubmitted_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Submitted, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be($"You have successfully submitted the evidence note with reference ID E{source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNoteStatusApproved_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Approved, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be(
                    $"You have approved the evidence note with reference ID E{ source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
         }

        [Fact]
        public void Map_GivenNoteStatusRejected_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Rejected, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be(
                    $"You have rejected the evidence note with reference ID E{ source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNoteStatusReturned_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Returned, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be(
                    $"You have returned the evidence note with reference ID E{ source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNoteStatusReturnedSaved_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.ReturnedSaved, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be($"You have successfully saved the returned evidence note with reference ID E{ source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNoteStatusReturnedSubmitted_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.ReturnedSubmitted, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be($"You have successfully submitted the returned evidence note with reference ID E{source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSubmittedDateTime_FormatsToGMTString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Submitted, TestFixture.Create<bool>());
            source.EvidenceNoteData.SubmittedDate = DateTime.Parse("01/01/2001 13:30:30");

            var result = map.Map(source);

            result.SubmittedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoSubmittedDateTime_FormatsToEmptyString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Submitted, TestFixture.Create<bool>());
            source.EvidenceNoteData.SubmittedDate = null;

            var result = map.Map(source);

            result.SubmittedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenNoSubmittedDateTime_SubmittedByShouldBeEmpty()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Submitted, TestFixture.Create<bool>());
            source.EvidenceNoteData.SubmittedDate = null;

            var result = map.Map(source);

            result.SubmittedBy.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenApprovedDateTime_FormatsToGMTString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Approved, TestFixture.Create<bool>());
            source.EvidenceNoteData.ApprovedDate = DateTime.Parse("21/01/2001 13:30:30");

            var result = map.Map(source);

            result.ApprovedDate.Should().Be("21/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoApprovedDateTime_FormatsToEmptyString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Approved, TestFixture.Create<bool>());
            source.EvidenceNoteData.ApprovedDate = null;

            var result = map.Map(source);

            result.ApprovedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenReturnedDateTime_FormatsToGMTString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Returned, TestFixture.Create<bool>());
            source.EvidenceNoteData.ReturnedDate = DateTime.Parse("01/01/2001 13:30:30");

            var result = map.Map(source);

            result.ReturnedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoReturnedDateTime_FormatsToEmptyString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Returned, TestFixture.Create<bool>());
            source.EvidenceNoteData.ReturnedDate = null;

            var result = map.Map(source);

            result.ReturnedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenSourceWithRedirectTab_RedirectTabShouldBeSet()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());

            var result = map.Map(source);

            result.RedirectTab.Should().Be(source.RedirectTab);
        }

        [Fact]
        public void Map_GivenSourceWithNoRedirectTab_RedirectTabShouldBeSet()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());
            source.RedirectTab = null;

            var result = map.Map(source);

            result.RedirectTab.Should().BeNull();
        }

        [Fact]
        public void Map_GivenReturnedReasonAndNoRejectedReason_ReasonMustBeSet()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Returned, TestFixture.Create<bool>());
            var reason = TestFixture.Create<string>();
            source.EvidenceNoteData.ReturnedReason = reason;
            source.EvidenceNoteData.RejectedReason = null;
            var result = map.Map(source);

            result.ReturnedReason.Should().Be(reason);
        }

        [Fact]
        public void Map_GivenNoReturnedReasonAndNoRejectedReason_ReasonMustBeNullOrEmpty()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Returned, TestFixture.Create<bool>());
            source.EvidenceNoteData.ReturnedReason = null;
            source.EvidenceNoteData.RejectedReason = null;

            var result = map.Map(source);

            result.ReturnedReason.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenRejectedDateTime_FormatsToGMTString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Rejected, TestFixture.Create<bool>());
            source.EvidenceNoteData.RejectedDate = DateTime.Parse("21/01/2001 13:30:30");

            var result = map.Map(source);

            result.RejectedDate.Should().Be("21/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoRejectedDateTime_FormatsToEmptyString()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Rejected, TestFixture.Create<bool>());
            source.EvidenceNoteData.RejectedDate = null;

            var result = map.Map(source);

            result.RejectedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenRejectedReason_ReasonMustBeSet()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Rejected, TestFixture.Create<bool>());
            var reason = TestFixture.Create<string>();
            source.EvidenceNoteData.RejectedReason = reason;
            var result = map.Map(source);

            result.RejectedReason.Should().Be(reason);
        }

        [Fact]
        public void Map_GivenNoRejectedReasonAndNoReturnedReason_ReasonMustBeNullOrEmpty()
        {
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Rejected, TestFixture.Create<bool>());
            source.EvidenceNoteData.RejectedReason = null;
            source.EvidenceNoteData.ReturnedReason = null;

            var result = map.Map(source);

            result.RejectedReason.Should().BeNullOrEmpty();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void Map_GivenSourceWhereNoteStatusIsNotDraftOrReturned_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Draft) || status.Equals(NoteStatus.Returned))
            {
                return;
            }

            //arrange
            var evidenceNoteData = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, status)
                .With(e => e.AatfData,
                    TestFixture.Build<AatfData>().With(a => a.CanCreateEditEvidence, true)
                        .Create()).Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.DisplayEditButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void Map_GivenSourceWhereNoteStatusIsDraftOrReturnedAndAatfCanEditEvidenceNotes_DisplayEditButtonShouldBeTrue(NoteStatus status)
        {
            if (!status.Equals(NoteStatus.Draft) && !status.Equals(NoteStatus.Returned))
            {
                return;
            }

            var evidenceNoteData = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, status)
                .With(e => e.AatfData,
                    TestFixture.Build<AatfData>().With(a => a.CanCreateEditEvidence, true)
                        .Create()).Create();
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.DisplayEditButton.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void Map_GivenSourceWhereNoteStatusIsDraftOrReturnedAndAatfCannotEditEvidenceNotes_DisplayEditButtonShouldBeFalse(NoteStatus status)
        {
            if (!status.Equals(NoteStatus.Draft) && !status.Equals(NoteStatus.Returned))
            {
                return;
            }

            var evidenceNoteData = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, status)
                .With(e => e.AatfData,
                    TestFixture.Build<AatfData>().With(a => a.CanCreateEditEvidence, false)
                        .Create()).Create();
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, TestFixture.Create<bool>());

            //act
            var result = map.Map(source);

            //assert
            result.DisplayEditButton.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithHistoryNoteData_EvidenceNoteHistoryViewModelShouldBePopulated()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(TestFixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft, TestFixture.Create<bool>());
            var data = new EvidenceNoteHistoryData(TestFixture.Create<Guid>(), TestFixture.Create<NoteStatus>(), TestFixture.Create<int>(), TestFixture.Create<NoteType>(), TestFixture.Create<DateTime?>(), TestFixture.Create<string>(), TestFixture.Create<List<EvidenceTonnageData>>());
            var history = new List<EvidenceNoteHistoryData>()
            {
                data
            };
            source.EvidenceNoteData.EvidenceNoteHistoryData = history;

            A.CallTo(() => mapper.Map<IList<EvidenceNoteRowViewModel>>(history)).Returns(new List<EvidenceNoteRowViewModel>()
            {
                new EvidenceNoteRowViewModel()
                {
                    Id = data.Id,
                    ReferenceId = data.Reference,
                    TransferredTo = data.TransferredTo,
                    Type = data.Type,
                    Status = data.Status,
                    SubmittedDate = data.SubmittedDate
                }
            });

            //act
            var result = map.Map(source);

            //assert
            result.EvidenceNoteHistoryData.First().Id.Should().Be(history.First().Id);
            result.EvidenceNoteHistoryData.First().Status.Should().Be(history.First().Status);
            result.EvidenceNoteHistoryData.First().ReferenceId.Should().Be(history.First().Reference);
            result.EvidenceNoteHistoryData.First().Type.Should().Be(history.First().Type);
            result.EvidenceNoteHistoryData.First().SubmittedDate.Should().Be(history.First().SubmittedDate);
            result.EvidenceNoteHistoryData.First().TransferredTo.Should().Be(history.First().TransferredTo);

            A.CallTo(() => mapper.Map<IList<EvidenceNoteRowViewModel>>(history)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void Map_GivenEvidenceNoteIsNotApprovedAndUserIsAdminAndNotPrintableVersion_CanVoidShouldBeFalse(NoteStatus status)
        {
            if (status == NoteStatus.Approved)
            {
                return;
            }

            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });
            
            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, status).Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsNotAdminAndNoTransferNotesInNonVoidableStateAndNotPrintableVersion_CanVoidShouldBeFalse()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { string.Empty });

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved).Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndNoTransferNotesAndNotPrintableVersion_CanVoidShouldBeTrue()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, new List<EvidenceNoteHistoryData>())
                .Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndTransferNotesInVoidStateAndNotPrintableVersion_CanVoidShouldBeTrue()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndTransferNotesInRejectedStateAndNotPrintableVersion_CanVoidShouldBeTrue()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndTransferNotesInRejectedAndVoidStateAndNotPrintableVersion_CanVoidShouldBeTrue()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Rejected,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndNoteHasTransferNotesInNotVoidableStateAndNotPrintableVersion_CanVoidShouldBeFalse(NoteStatus status)
        {
            if (status == NoteStatus.Void || status == NoteStatus.Rejected)
            {
                return;
            }

            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    status,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndNoteHasNoTransfersInNonVoidableStatusAndNotPrintableVersion_CanVoidShouldBeTrue()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Rejected,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, false, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIsApprovedAndUserIsAdminAndNoteHasNoTransfersInNonVoidableStatusAndPrintableVersion_CanVoidShouldBeFalse()
        {
            //arrange
            var user = new GenericPrincipal(A.Fake<IIdentity>(), new[] { Claims.InternalAdmin });

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Rejected,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.Status, NoteStatus.Approved)
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, true, user);

            //act
            var result = map.Map(source);

            //assert
            result.CanVoid.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void Map_GivenEvidenceNoteHasNotesThatAreNotVoidOrRejected_CanDisplayNotesMessageShouldBeTrue(NoteStatus status)
        {
            if (status == NoteStatus.Void || status == NoteStatus.Rejected)
            {
                return;
            }

            //arrange
            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    status,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.CanDisplayNotesMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenEvidenceNoteHasOnlyVoidedNotes_CanDisplayNotesMessageShouldBeFalse()
        {
            //arrange
            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.CanDisplayNotesMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenEvidenceNoteHasOnlyRejectedNotes_CanDisplayNotesMessageShouldBeFalse()
        {
            //arrange
            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Rejected,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Rejected,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.CanDisplayNotesMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenEvidenceNoteHasVoidedAndRejectedNotes_CanDisplayNotesMessageShouldBeFalse()
        {
            //arrange
            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Void,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>()),
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Rejected,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    TestFixture.Create<List<EvidenceTonnageData>>())
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.CanDisplayNotesMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenApprovedTransferHistoryNotes_TransferTonnagesAndDisplayTransferTonnages_AreMapped()
        {
            //arrange
            var tonnageHistory = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 10, 10, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.SmallHouseholdAppliances, 10, 10, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, 10, 5, null, null),
            };

            var currentTonnage = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 20, 20, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.SmallHouseholdAppliances, 20, 20, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, 20, 10, null, null),
            };

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Approved,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    tonnageHistory)
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .With(e => e.EvidenceTonnageData, currentTonnage)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(10m)).Returns("10.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(5m)).Returns("5.000");

            //act
            var result = map.Map(source);

            //assert
            result.DisplayTransferEvidenceColumns.Should().BeTrue();
            result.RemainingTransferCategoryValues.FirstOrDefault(x => (WeeeCategory)x.CategoryId == WeeeCategory.LargeHouseholdAppliances)?.Received.Should().Be("10.000");
            result.RemainingTransferCategoryValues.FirstOrDefault(x => (WeeeCategory)x.CategoryId == WeeeCategory.LargeHouseholdAppliances)?.Reused.Should().Be("10.000");
            result.RemainingTransferCategoryValues.FirstOrDefault(x => (WeeeCategory)x.CategoryId == WeeeCategory.SmallHouseholdAppliances)?.Received.Should().Be("10.000");
            result.RemainingTransferCategoryValues.FirstOrDefault(x => (WeeeCategory)x.CategoryId == WeeeCategory.SmallHouseholdAppliances)?.Reused.Should().Be("10.000");
            result.RemainingTransferCategoryValues.FirstOrDefault(x => (WeeeCategory)x.CategoryId == WeeeCategory.ITAndTelecommsEquipment)?.Received.Should().Be("10.000");
            result.RemainingTransferCategoryValues.FirstOrDefault(x => (WeeeCategory)x.CategoryId == WeeeCategory.ITAndTelecommsEquipment)?.Reused.Should().Be("5.000");
            result.TransferReceivedRemainingTotalDisplay.Should().Be("30.000");
            result.TransferReusedRemainingTotalDisplay.Should().Be("25.000");
        }

        [Fact]
        public void Map_GivenNoApprovedTransferHistoryNotes_TransferTonnagesAndDisplayTransferTonnages_AreMappedAsFalse()
        {
            //arrange
            var tonnageHistory = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 10, 10, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.SmallHouseholdAppliances, 10, 10, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, 10, 10, null, null),
            };

            var currentTonnage = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 20, 20, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.SmallHouseholdAppliances, 20, 20, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, 20, 10, null, null),
            };

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Submitted,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    tonnageHistory)
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .With(e => e.EvidenceTonnageData, currentTonnage)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.DisplayTransferEvidenceColumns.Should().BeFalse();
            result.TransferReceivedRemainingTotalDisplay.Should().Be("0.000");
            result.TransferReusedRemainingTotalDisplay.Should().Be("0.000");
        }

        [Fact]
        public void Map_GivenApprovedTransferHistoryNotes_ButCategoryHasZeroTransfer_TransferTonnagesAndDisplayTransferTonnages_AreMappedAsOriginalAmount()
        {
            //arrange
            var tonnageHistory = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 20, 20, null, null),
            };

            var currentTonnage = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 20, 20, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.SmallHouseholdAppliances, 20, 20, null, null),
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, 20, 10, null, null),
            };

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Approved,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    tonnageHistory)
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .With(e => e.EvidenceTonnageData, currentTonnage)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(0m)).Returns("0.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(10m)).Returns("10.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(20m)).Returns("20.000");

            //act
            var result = map.Map(source);

            //assert
            result.DisplayTransferEvidenceColumns.Should().BeTrue();
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.LargeHouseholdAppliances).FirstOrDefault().Received.Should().Be("0.000");
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.LargeHouseholdAppliances).FirstOrDefault().Reused.Should().Be("0.000");
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.SmallHouseholdAppliances).FirstOrDefault().Received.Should().Be("20.000");
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.SmallHouseholdAppliances).FirstOrDefault().Reused.Should().Be("20.000");
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.ITAndTelecommsEquipment).FirstOrDefault().Received.Should().Be("20.000");
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.ITAndTelecommsEquipment).FirstOrDefault().Reused.Should().Be("10.000");
            result.TransferReceivedRemainingTotalDisplay.Should().Be("40.000");
            result.TransferReusedRemainingTotalDisplay.Should().Be("30.000");
        }

        [Fact]
        public void Map_GivenApprovedTransferHistoryNotes_AndIsFullyTransferred_TransferTonnagesAndDisplayTransferTonnages_ShouldDisplayZero()
        {
            //arrange
            var tonnageHistory = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 20, 20, null, null),
            };

            var currentTonnage = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.LargeHouseholdAppliances, 20, 20, null, null),
            };

            var evidenceNoteHistory = new List<EvidenceNoteHistoryData>()
            {
                new EvidenceNoteHistoryData(TestFixture.Create<Guid>(),
                    NoteStatus.Approved,
                    TestFixture.Create<int>(),
                    NoteType.Transfer,
                    TestFixture.Create<DateTime>(),
                    TestFixture.Create<string>(),
                    tonnageHistory)
            };

            var evidenceNote = TestFixture.Build<EvidenceNoteData>()
                .With(e => e.EvidenceNoteHistoryData, evidenceNoteHistory)
                .With(e => e.EvidenceTonnageData, currentTonnage)
                .Create();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNote, null, TestFixture.Create<bool>(), null);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(0m)).Returns("0.000");

            //act
            var result = map.Map(source);

            //assert
            result.DisplayTransferEvidenceColumns.Should().BeTrue();
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.LargeHouseholdAppliances).FirstOrDefault().Received.Should().Be("0.000");
            result.RemainingTransferCategoryValues.Where(x => (WeeeCategory)x.CategoryId == WeeeCategory.LargeHouseholdAppliances).FirstOrDefault().Reused.Should().Be("0.000");
            result.TransferReceivedRemainingTotalDisplay.Should().Be("0.000");
            result.TransferReusedRemainingTotalDisplay.Should().Be("0.000");
        }

        [Fact]
        public void ViewEvidenceNoteViewModelMap_GivenSourceWithNullUser_ViewEvidenceNoteViewModelIsInternalUserShouldBeFalse()
        {
            //arrange
            var evidenceNoteData = TestFixture.Create<EvidenceNoteData>();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, null);

            //act
            var model = map.Map(source);

            //assert
            model.IsInternalUser.Should().BeFalse();
        }

        [Fact]
        public void ViewEvidenceNoteViewModelMap_GivenSourceWithUserThatDoesNotHaveInternalUserClaim_ViewEvidenceNoteViewModelIsInternalUserShouldBeFalse()
        {
            //arrange
            var evidenceNoteData = TestFixture.Create<EvidenceNoteData>();

            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, A.Fake<IPrincipal>());

            //act
            var model = map.Map(source);

            //assert
            model.IsInternalUser.Should().BeFalse();
        }

        [Fact]
        public void ViewEvidenceNoteViewModelMap_GivenSourceWithUserThatDoesHaveInternalUserClaim_ViewEvidenceNoteViewModelIsInternalUserShouldBeTrue()
        {
            //arrange
            var evidenceNoteData = TestFixture.Create<EvidenceNoteData>();
            var identity = new ClaimsIdentity();
            var user = A.Fake<IPrincipal>();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea));
            A.CallTo(() => user.Identity).Returns(identity);

            var source = new ViewEvidenceNoteMapTransfer(evidenceNoteData, null, false, user);

            //act
            var model = map.Map(source);

            //assert
            model.IsInternalUser.Should().BeTrue();
        }
    }
}
