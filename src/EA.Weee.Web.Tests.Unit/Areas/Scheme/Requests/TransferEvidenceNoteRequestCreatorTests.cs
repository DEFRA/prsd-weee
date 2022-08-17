namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Web.Areas.Scheme.Requests;
    using Web.Extensions;
    using Web.ViewModels.Shared;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNoteRequestCreatorTests : SimpleUnitTestBase
    {
        private readonly TransferEvidenceNoteRequestCreator requestCreator;

        public TransferEvidenceNoteRequestCreatorTests()
        {
            requestCreator = new TransferEvidenceNoteRequestCreator();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenNullViewModel_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => requestCreator.SelectCategoriesToRequest(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenSelectedSchemeIsNull_InvalidOperationExceptionExpected()
        {
            //arrange
            var model = GetModel();
            model.SelectedSchema = null;

            //act
            var exception = Record.Exception(() => requestCreator.SelectCategoriesToRequest(model));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenThereIsNoCategorySelected_InvalidOperationExceptionExpected()
        {
            //arrange
            var model = GetModel();
           
            //act
            var exception = Record.Exception(() => requestCreator.SelectCategoriesToRequest(model));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenValidModel_CreateTransferNoteRequestShouldBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();

            var model = GetValidModelWithSelectedCategories(selectedScheme, organisationId);
            var selectedIds = model.CategoryBooleanViewModels.Where(x => x.Selected).Select(x => x.CategoryId).ToList();

           //act
           var request = requestCreator.SelectCategoriesToRequest(model);

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.RecipientId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNoteIds.Should().BeNullOrEmpty();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenValidModelAndExistingRequestWithNullEvidenceNotes_CreateTransferNoteRequestShouldBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();

            var model = GetValidModelWithSelectedCategories(selectedScheme, organisationId);
            var selectedIds = model.CategoryBooleanViewModels.Where(x => x.Selected).Select(x => x.CategoryId).ToList();

            //act
            var request = requestCreator.SelectCategoriesToRequest(model, new TransferEvidenceNoteRequest());

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.RecipientId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNoteIds.Should().BeEmpty();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenValidModelAndExistingRequestWithEmptyEvidenceNotes_CreateTransferNoteRequestShouldBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();

            var model = GetValidModelWithSelectedCategories(selectedScheme, organisationId);
            var selectedIds = model.CategoryBooleanViewModels.Where(x => x.Selected).Select(x => x.CategoryId).ToList();

            //act
            var request = requestCreator.SelectCategoriesToRequest(model, new TransferEvidenceNoteRequest());

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.RecipientId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNoteIds.Should().BeEmpty();
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenValidModelAndExistingRequestWithEvidenceNotes_CreateTransferNoteRequestShouldBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();

            var model = GetValidModelWithSelectedCategories(selectedScheme, organisationId);
            var selectedIds = model.CategoryBooleanViewModels.Where(x => x.Selected).Select(x => x.CategoryId).ToList();
            var evidenceIds = TestFixture.CreateMany<Guid>().ToList();

            var existingRequest =
                new TransferEvidenceNoteRequest(organisationId, selectedScheme, selectedIds, evidenceIds);

            //act
            var request = requestCreator.SelectCategoriesToRequest(model, existingRequest);

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.RecipientId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNoteIds.Should().BeEquivalentTo(evidenceIds);
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenValidModelAndExistingRequestWithExcludeEvidenceNotes_CreateTransferNoteRequestShouldBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();

            var model = GetValidModelWithSelectedCategories(selectedScheme, organisationId);
            var selectedIds = model.CategoryBooleanViewModels.Where(x => x.Selected).Select(x => x.CategoryId).ToList();
            var evidenceIds = TestFixture.CreateMany<Guid>().ToList();
            var excludedEvidenceIds = TestFixture.CreateMany<Guid>().ToList();

            var existingRequest =
                new TransferEvidenceNoteRequest(organisationId, selectedScheme, selectedIds, evidenceIds, excludedEvidenceIds);

            //act
            var request = requestCreator.SelectCategoriesToRequest(model, existingRequest);

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.RecipientId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNoteIds.Should().BeEquivalentTo(evidenceIds);
            request.ExcludeEvidenceNoteIds.Should().BeEquivalentTo(excludedEvidenceIds);
        }

        [Fact]
        public void SelectCategoriesToRequest_GivenValidModelAndExistingRequestWithEmptyExcludeEvidenceNotes_CreateTransferNoteRequestShouldBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();

            var model = GetValidModelWithSelectedCategories(selectedScheme, organisationId);
            var selectedIds = model.CategoryBooleanViewModels.Where(x => x.Selected).Select(x => x.CategoryId).ToList();

            //act
            var request = requestCreator.SelectCategoriesToRequest(model, new TransferEvidenceNoteRequest());

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.RecipientId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNoteIds.Should().BeEmpty();
            request.ExcludeEvidenceNoteIds.Should().BeEmpty();
        }

        [Theory]
        [InlineData(ActionEnum.Save, NoteStatus.Draft)]
        [InlineData(ActionEnum.Submit, NoteStatus.Submitted)]
        public void SelectTonnageToRequest_RequestAndViewModel_RequestShouldBeCreated(ActionEnum action, NoteStatus expectedStatus)
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var schemeId = TestFixture.Create<Guid>();
            var categories = TestFixture.CreateMany<int>().ToList();
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();
            var complianceYear = TestFixture.Create<int>();

            var transferCategoryTonnage = new List<TransferEvidenceCategoryValue>()
            {
                new TransferEvidenceCategoryValue(WeeeCategory.ConsumerEquipment, Guid.NewGuid(), 2, 1, "2", "1"),
                new TransferEvidenceCategoryValue(WeeeCategory.DisplayEquipment, Guid.NewGuid(), null, null, null, null)
            };

            var request = new TransferEvidenceNoteRequest(organisationId,
                schemeId,
                categories,
                evidenceNoteIds);

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(v => v.Action, action)
                .With(v => v.TransferCategoryValues, transferCategoryTonnage)
                .With(v => v.ComplianceYear, complianceYear)
                .Create();
            
            //act
            var result = requestCreator.SelectTonnageToRequest(request, model);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.RecipientId.Should().Be(schemeId);
            result.Status.Should().Be(expectedStatus);
            result.CategoryIds.Should().BeEquivalentTo(categories);
            result.EvidenceNoteIds.Should().BeEquivalentTo(evidenceNoteIds);
            result.ComplianceYear.Should().Be(complianceYear);
            var category1ToFind = transferCategoryTonnage.ElementAt(0);
            result.TransferValues.Should().Contain(t =>
                t.Id.Equals(category1ToFind.Id) &&
                t.CategoryId.Equals(category1ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category1ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category1ToFind.Reused.ToDecimal()));
            var category2ToFind = transferCategoryTonnage.ElementAt(1);
            result.TransferValues.Should().Contain(t =>
                t.Id.Equals(category2ToFind.Id) &&
                t.CategoryId.Equals(category2ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category2ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category2ToFind.Reused.ToDecimal()));
        }

        [Theory]
        [InlineData(ActionEnum.Save, NoteStatus.Draft)]
        [InlineData(ActionEnum.Submit, NoteStatus.Submitted)]
        public void EditSelectTonnageToRequest_ViewModelWithNullExistingRequest_RequestShouldBeCreated(ActionEnum action, NoteStatus expectedStatus)
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var schemeId = TestFixture.Create<Guid>();
            var evidenceNoteId = TestFixture.Create<Guid>();
            var electricalAndElectronicToolsTonnageId = TestFixture.Create<Guid>();

            var transferCategoryTonnage = new List<TransferEvidenceCategoryValue>()
            {
                new TransferEvidenceCategoryValue(WeeeCategory.ConsumerEquipment, Guid.NewGuid(), 2, 1, "2", "1"),
                new TransferEvidenceCategoryValue(WeeeCategory.DisplayEquipment, Guid.NewGuid(), null, null, null, null),
                new TransferEvidenceCategoryValue(WeeeCategory.ElectricalAndElectronicTools, electricalAndElectronicToolsTonnageId, 4, 2, "3", "2")
            };

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(v => v.Action, action)
                .With(v => v.TransferCategoryValues, transferCategoryTonnage)
                .With(v => v.PcsId, organisationId)
                .With(v => v.RecipientId, schemeId)
                .With(v => v.ViewTransferNoteViewModel, TestFixture.Build<ViewTransferNoteViewModel>().With(vt => vt.EvidenceNoteId, evidenceNoteId).Create())
                .Create();

            //act
            var result = requestCreator.EditSelectTonnageToRequest(null, model);

            //assert
            result.TransferNoteId.Should().Be(evidenceNoteId);
            result.OrganisationId.Should().Be(organisationId);
            result.RecipientId.Should().Be(schemeId);
            result.Status.Should().Be(expectedStatus);
            result.CategoryIds.Should().BeNull();
            result.EvidenceNoteIds.Should().BeEmpty();
            var category1ToFind = transferCategoryTonnage.ElementAt(0);
            result.TransferValues.Should().Contain(t =>
                t.TransferTonnageId.Equals(category1ToFind.TransferTonnageId) &&
                t.CategoryId.Equals(category1ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category1ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category1ToFind.Reused.ToDecimal()));
            var category2ToFind = transferCategoryTonnage.ElementAt(1);
            result.TransferValues.Should().Contain(t =>
                t.TransferTonnageId.Equals(category2ToFind.TransferTonnageId) &&
                t.CategoryId.Equals(category2ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category2ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category2ToFind.Reused.ToDecimal()));
            var category3ToFind = transferCategoryTonnage.ElementAt(2);
            result.TransferValues.Should().Contain(t =>
                t.TransferTonnageId.Equals(category3ToFind.TransferTonnageId) &&
                t.CategoryId.Equals(category3ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category3ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category3ToFind.Reused.ToDecimal()));
        }

        [Theory]
        [InlineData(ActionEnum.Save, NoteStatus.Draft)]
        [InlineData(ActionEnum.Submit, NoteStatus.Submitted)]
        public void EditSelectTonnageToRequest_ViewModelAndExistingRequest_RequestShouldBeCreated(ActionEnum action, NoteStatus expectedStatus)
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var schemeId = TestFixture.Create<Guid>();
            var evidenceNoteId = TestFixture.Create<Guid>();
            var electricalAndElectronicToolsTonnageId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var transferCategoryTonnage = new List<TransferEvidenceCategoryValue>()
            {
                new TransferEvidenceCategoryValue(WeeeCategory.ConsumerEquipment, Guid.NewGuid(), 2, 1, "2", "1"),
                new TransferEvidenceCategoryValue(WeeeCategory.DisplayEquipment, Guid.NewGuid(), null, null, null, null),
                new TransferEvidenceCategoryValue(WeeeCategory.ElectricalAndElectronicTools, electricalAndElectronicToolsTonnageId, 4, 2, "3", "2")
            };

            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(v => v.Action, action)
                .With(v => v.TransferCategoryValues, transferCategoryTonnage)
                .With(v => v.PcsId, organisationId)
                .With(v => v.ViewTransferNoteViewModel, TestFixture.Build<ViewTransferNoteViewModel>().With(vt => vt.EvidenceNoteId, evidenceNoteId).Create())
                .Create();

            var request = new TransferEvidenceNoteRequest(TestFixture.Create<Guid>(),
                schemeId,
                TestFixture.CreateMany<int>().ToList(),
                TestFixture.CreateMany<Guid>().ToList());

            //act
            var result = requestCreator.EditSelectTonnageToRequest(request, model);

            //assert
            result.TransferNoteId.Should().Be(evidenceNoteId);
            result.OrganisationId.Should().Be(organisationId);
            result.RecipientId.Should().Be(schemeId);
            result.Status.Should().Be(expectedStatus);
            result.CategoryIds.Should().BeNull();
            result.EvidenceNoteIds.Should().BeEmpty();
            var category1ToFind = transferCategoryTonnage.ElementAt(0);
            result.TransferValues.Should().Contain(t =>
                t.TransferTonnageId.Equals(category1ToFind.TransferTonnageId) &&
                t.CategoryId.Equals(category1ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category1ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category1ToFind.Reused.ToDecimal()));
            var category2ToFind = transferCategoryTonnage.ElementAt(1);
            result.TransferValues.Should().Contain(t =>
                t.TransferTonnageId.Equals(category2ToFind.TransferTonnageId) &&
                t.CategoryId.Equals(category2ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category2ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category2ToFind.Reused.ToDecimal()));
            var category3ToFind = transferCategoryTonnage.ElementAt(2);
            result.TransferValues.Should().Contain(t =>
                t.TransferTonnageId.Equals(category3ToFind.TransferTonnageId) &&
                t.CategoryId.Equals(category3ToFind.CategoryId) &&
                t.FirstTonnage.Equals(category3ToFind.Received.ToDecimal()) &&
                t.SecondTonnage.Equals(category3ToFind.Reused.ToDecimal()));
        }

        private TransferEvidenceNoteCategoriesViewModel GetModel()
        {
            return new TransferEvidenceNoteCategoriesViewModel();
        }

        private TransferEvidenceNoteCategoriesViewModel GetValidModelWithSelectedCategories(Guid selectedScheme, Guid organisationId)
        {
            var model = GetModel();
            model.CategoryBooleanViewModels.ElementAt(0).Selected = true;
            model.CategoryBooleanViewModels.ElementAt(1).Selected = true;
            model.CategoryBooleanViewModels.ElementAt(2).Selected = true;
            model.SelectedSchema = selectedScheme;
            model.OrganisationId = organisationId;

            return model;
        }
    }
}
