namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using Core.Organisations;
    using FakeItEasy;
    using FluentAssertions;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Web.ViewModels.Shared.Utilities;
    using Xunit;

    public class ViewEvidenceNoteViewModelMapTests
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;
        private readonly ViewEvidenceNoteViewModelMap map;
        private readonly Fixture fixture;

        public ViewEvidenceNoteViewModelMapTests()
        {
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            addressUtilities = A.Fake<IAddressUtilities>();
            fixture = new Fixture();

            map = new ViewEvidenceNoteViewModelMap(tonnageUtilities, addressUtilities);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var result = Record.Exception(() => map.Map(null));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_StandardPropertiesShouldBeMapped()
        {
            //arrange
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();

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
            result.SelectedComplianceYear.Should().Be(source.SelectedComplianceYear);
            result.ComplianceYear.Should().Be(source.EvidenceNoteData.ComplianceYear);
        }

        [Fact]
        public void Map_GivenSource_OperatorAddressShouldBeSet()
        {
            //arrange
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
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
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
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
        public void Map_GivenSourceWithRecipientThatHasBusinessAddress_RecipientAddressShouldBeSetToBusinessAddress()
        {
            //arrange
            var organisation = fixture.Build<OrganisationData>()
                .With(o => o.HasBusinessAddress, true)
                .With(o => o.OrganisationName, "org").Create();
            var evidenceData = fixture.Build<EvidenceNoteData>()
                .With(e => e.RecipientOrganisationData, organisation)
                .Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceData, null);
            
            const string recipientAddress = "recipientAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.SchemeData.SchemeName,
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
            result.RecipientAddress.Should().Be(recipientAddress);
        }

        [Fact]
        public void Map_GivenSourceWithRecipientThatDoesNotHaveBusinessAddress_RecipientAddressShouldBeSetToNotificationAddress()
        {
            //arrange
            var organisation = fixture.Build<OrganisationData>()
                .With(o => o.HasBusinessAddress, false)
                .With(o => o.OrganisationName, "org").Create();
            var evidenceData = fixture.Build<EvidenceNoteData>()
                .With(e => e.RecipientOrganisationData, organisation)
                .Create();
            var source = new ViewEvidenceNoteMapTransfer(evidenceData, null);

            const string recipientAddress = "recipientAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.SchemeData.SchemeName,
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
        public void Map_GivenTonnagesAndIncludeAllCategories_TonnagesShouldBeFormatted()
        {
            //arrange
            var source = fixture.Build<ViewEvidenceNoteMapTransfer>().With(v => v.IncludeAllCategories, true).Create();

            source.EvidenceNoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ConsumerEquipment, null, 1),
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ElectricalAndElectronicTools, 2, null)
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
            var source = fixture.Build<ViewEvidenceNoteMapTransfer>().With(v => v.IncludeAllCategories, false).Create();

            source.EvidenceNoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ElectricalAndElectronicTools, 2, null)
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
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();

            source.EvidenceNoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.ConsumerEquipment, 1, 5),
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.ElectricalAndElectronicTools, 2, null),
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.GasDischargeLampsAndLedLightSources, 3, 20),
                new EvidenceTonnageData(Guid.Empty, WeeeCategory.ITAndTelecommsEquipment, null, 50)
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
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.DisplayMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenNoteStatusDraftCreated_SuccessMessageShouldBeShown()
        {
            //arrange
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Draft);

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
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Submitted);

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
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Approved);

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
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Rejected);

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
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.Returned);

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
            var source = new ViewEvidenceNoteMapTransfer(fixture.Create<EvidenceNoteData>(), NoteUpdatedStatusEnum.ReturnedSaved);

            //act
            var result = map.Map(source);

            //assert
            result.SuccessMessage.Should()
                .Be($"You have successfully saved the returned evidence note with reference ID E{ source.EvidenceNoteData.Reference}");
            result.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSubmittedDateTime_FormatsToGMTString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.SubmittedDate = DateTime.Parse("01/01/2001 13:30:30");

            var result = map.Map(source);

            result.SubmittedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoSubmittedDateTime_FormatsToEmptyString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.SubmittedDate = null;

            var result = map.Map(source);

            result.SubmittedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenNoSubmittedDateTime_SubmittedByShouldBeEmpty()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.SubmittedDate = null;

            var result = map.Map(source);

            result.SubmittedBy.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenApprovedDateTime_FormatsToGMTString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.ApprovedDate = DateTime.Parse("01/01/2001 13:30:30");

            var result = map.Map(source);

            result.ApprovedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoApprovedDateTime_FormatsToEmptyString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.ApprovedDate = null;

            var result = map.Map(source);

            result.ApprovedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenReturnedDateTime_FormatsToGMTString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.ReturnedDate = DateTime.Parse("01/01/2001 13:30:30");

            var result = map.Map(source);

            result.ReturnedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoReturnedDateTime_FormatsToEmptyString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.ReturnedDate = null;

            var result = map.Map(source);

            result.ReturnedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenReturnedReasonAndNoRejectedReason_ReasonMustBeSet()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            var reason = fixture.Create<string>();
            source.EvidenceNoteData.ReturnedReason = reason;
            source.EvidenceNoteData.RejectedReason = null;
            var result = map.Map(source);

            result.ReturnedReason.Should().Be(reason);
        }

        [Fact]
        public void Map_GivenNoReturnedReasonAndNoRejectedReason_ReasonMustBeNullOrEmpty()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.ReturnedReason = null;
            source.EvidenceNoteData.RejectedReason = null;

            var result = map.Map(source);

            result.ReturnedReason.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenRejectedDateTime_FormatsToGMTString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.RejectedDate = DateTime.Parse("01/01/2001 13:30:30");

            var result = map.Map(source);

            result.RejectedDate.Should().Be("01/01/2001 13:30:30 (GMT)");
        }

        [Fact]
        public void Map_GivenNoRejectedDateTime_FormatsToEmptyString()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.RejectedDate = null;

            var result = map.Map(source);

            result.RejectedDate.Should().Be(string.Empty);
        }

        [Fact]
        public void Map_GivenRejectedReason_ReasonMustBeSet()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            var reason = fixture.Create<string>();
            source.EvidenceNoteData.RejectedReason = reason;
            var result = map.Map(source);

            result.RejectedReason.Should().Be(reason);
        }

        [Fact]
        public void Map_GivenNoRejectedReasonAndNoReturnedReason_ReasonMustBeNullOrEmpty()
        {
            var source = fixture.Create<ViewEvidenceNoteMapTransfer>();
            source.EvidenceNoteData.RejectedReason = null;
            source.EvidenceNoteData.ReturnedReason = null;

            var result = map.Map(source);

            result.RejectedReason.Should().BeNullOrEmpty();
        }

        private ViewEvidenceNoteMapTransfer SetupTonnage(bool includeAll)
        {
            var source = fixture.Build<ViewEvidenceNoteMapTransfer>().With(v => v.IncludeAllCategories, includeAll).Create();

            source.EvidenceNoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ConsumerEquipment, null, 1),
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ElectricalAndElectronicTools, 2, null)
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
            return source;
        }
    }
}
