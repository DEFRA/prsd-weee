namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Xunit;

    public class ViewTransferNoteViewModelMapTests
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly Fixture fixture;
        private readonly ViewTransferNoteViewModelMap map;

        public ViewTransferNoteViewModelMapTests()
        {
            addressUtilities = A.Fake<IAddressUtilities>();
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            fixture = new Fixture();
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
            var source = fixture.Create<ViewTransferNoteViewModelMapTransfer>();

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
            var source = fixture.Build<ViewTransferNoteViewModelMapTransfer>().With(t => t.ReturnToView, returnToView).Create();

            //act
            var model = map.Map(source);

            //assert
            model.ReturnToView.Should().BeFalse();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSourceWithReturnToViewAsTrue_ShouldSetReturnToView()
        {
            //arrange
            var source = fixture.Build<ViewTransferNoteViewModelMapTransfer>().With(t => t.ReturnToView, true).Create();

            //act
            var model = map.Map(source);

            //assert
            model.ReturnToView.Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ViewTransferNoteViewModelMap_GivenSource_PropertiesShouldBeSet(bool editMode)
        {
            //arrange
            var source = fixture.Build<ViewTransferNoteViewModelMapTransfer>().With(t => t.Edit, editMode).Create();

            //act
            var model = map.Map(source);

            //assert
            model.SchemeId.Should().Be(source.SchemeId);
            model.EvidenceNoteId.Should().Be(source.TransferEvidenceNoteData.Id);
            model.SelectedComplianceYear.Should().Be(source.SelectedComplianceYear);
            model.EditMode.Should().Be(editMode);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSourceNoteType_PropertiesShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                    fixture.Build<TransferEvidenceNoteData>().With(t1 => t1.Type, NoteType.Transfer).Create(),
                    null);

            //act
            var model = map.Map(source);

            //assert
            model.Type.Should().Be(source.TransferEvidenceNoteData.Type);
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenSourceNoteStatus_PropertiesShouldBeSet(NoteStatus status)
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>().With(t1 => t1.Status, status).Create(),
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
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Status, NoteStatus.Draft)
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                true);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully saved the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference} as a draft");
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsSubmitted_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Status, NoteStatus.Submitted)
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                true);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully submitted the evidence note transfer with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference}");
        }

        [Theory]
        [InlineData(null, NoteStatus.Draft)]
        [InlineData(false, NoteStatus.Draft)]
        [InlineData(null, NoteStatus.Submitted)]
        [InlineData(false, NoteStatus.Submitted)]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationIsFalse_SuccessMessageShouldBeEmpty(bool? display, NoteStatus status)
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
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
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
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
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageData()).Create(),
                false);
            source.ReturnToView = true;

            //act
            var model = map.Map(source);

            //assert
            model.ComplianceYear.Should().Be(source.TransferEvidenceNoteData.ComplianceYear);
            model.ReturnToView.Should().Be(source.ReturnToView.Value);
            model.EditMode.Should().Be(source.Edit);
            model.SelectedComplianceYear.Should().Be(source.SelectedComplianceYear);
            model.Reference.Should().Be(source.TransferEvidenceNoteData.Reference);
            model.Type.Should().Be(source.TransferEvidenceNoteData.Type);
            model.Status.Should().Be(source.TransferEvidenceNoteData.Status);
            model.SchemeId.Should().Be(source.SchemeId);
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
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData()).Create(),
                false);
            const string siteAddress = "siteAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.RecipientSchemeData.SchemeName,
                source.TransferEvidenceNoteData.RecipientOrganisationData.OrganisationName,
                source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.Address1,
                source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.Address2,
                source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.TownOrCity,
                source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.CountyOrRegion,
                source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress.Postcode,
                null)).Returns(siteAddress);

            //act
            var result = map.Map(source);

            //assert
            result.RecipientAddress.Should().Be(siteAddress);
        }

        [Fact]
        public void Map_GivenSource_TransferredByAddressShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .Create(),
                false);
            const string siteAddress = "siteAddress";

            A.CallTo(() => addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.TransferredSchemeData.SchemeName,
                source.TransferEvidenceNoteData.TransferredOrganisationData.OrganisationName,
                source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.Address1,
                source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.Address2,
                source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.TownOrCity,
                source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.CountyOrRegion,
                source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress.Postcode,
                null)).Returns(siteAddress);

            //act
            var result = map.Map(source);

            //assert
            result.TransferredByAddress.Should().Be(siteAddress);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenTransfer_CorrectlySumsAndOrdersTotalCategories()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
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
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
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
        public void ViewTransferNoteViewModelMap_GivenTransfer_WhenGeneratingAatfDataViewModel_CallsTonnageUtilities()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(x => x.TransferredOrganisationData, CreateOrganisationData())
                    .With(x => x.RecipientOrganisationData, CreateOrganisationData())
                    .With(x => x.TransferEvidenceNoteTonnageData, CreateTransferEvidenceNoteTonnageData()).Create(),
                false);

            //act
            var model = map.Map(source);

            //assert
            //Two times per TransferEvidenceNoteTonnageData
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(A<decimal?>._)).MustHaveHappened(18, Times.Exactly);
        }

        private OrganisationData CreateOrganisationData()
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
                }
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

            return new List<TransferEvidenceNoteTonnageData>() { noteCat1, noteCat2, noteCat3, note2Cat1, note2Cat2, note2Cat3, note3Cat1, note3Cat2, note3Cat3 };
        }
    }
}
