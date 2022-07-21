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
    using Xunit;

    public class TransferEvidenceNoteRequestCreatorTests
    {
        private readonly TransferEvidenceNoteRequestCreator requestCreator;
        private readonly Fixture fixture;

        public TransferEvidenceNoteRequestCreatorTests()
        {
            fixture = new Fixture();

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
        }

        [Theory]
        [InlineData(ActionEnum.Save, NoteStatus.Draft)]
        [InlineData(ActionEnum.Submit, NoteStatus.Submitted)]
        public void SelectTonnageToRequest_RequestAndViewModel_RequestShouldBeCreated(ActionEnum action, NoteStatus expectedStatus)
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            var schemeId = fixture.Create<Guid>();
            var categories = fixture.CreateMany<int>().ToList();
            var evidenceNoteIds = fixture.CreateMany<Guid>().ToList();

            var transferCategoryTonnage = new List<TransferEvidenceCategoryValue>()
            {
                new TransferEvidenceCategoryValue(WeeeCategory.ConsumerEquipment, Guid.NewGuid(), 2, 1, "2", "1"),
                new TransferEvidenceCategoryValue(WeeeCategory.DisplayEquipment, Guid.NewGuid(), null, null, null, null)
            };

            var request = new TransferEvidenceNoteRequest(organisationId,
                schemeId,
                categories,
                evidenceNoteIds);

            var model = fixture.Build<TransferEvidenceTonnageViewModel>()
                .With(v => v.Action, action)
                .With(v => v.TransferCategoryValues, transferCategoryTonnage)
                .Create();
            
            //act
            var result = requestCreator.SelectTonnageToRequest(request, model);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.RecipientId.Should().Be(schemeId);
            result.Status.Should().Be(expectedStatus);
            result.CategoryIds.Should().BeEquivalentTo(categories);
            result.EvidenceNoteIds.Should().BeEquivalentTo(evidenceNoteIds);
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
        }

        [Fact]

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
