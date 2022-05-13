namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Requests
{
    using System;
    using System.Linq;
    using AutoFixture;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Web.Areas.Scheme.Requests;
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
            var selectedIds = model.CategoryValues.Where(x => x.Selected).Select(x => x.CategoryId).ToList();

           //act
           var request = requestCreator.SelectCategoriesToRequest(model);

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.SchemeId.Should().Be(selectedScheme);
            request.OrganisationId.Should().Be(organisationId);
        }

        private TransferEvidenceNoteCategoriesViewModel GetModel()
        {
            return new TransferEvidenceNoteCategoriesViewModel();
        }

        private TransferEvidenceNoteCategoriesViewModel GetValidModelWithSelectedCategories(Guid selectedScheme, Guid organisationId)
        {
            var model = GetModel();
            model.CategoryValues.ElementAt(0).Selected = true;
            model.CategoryValues.ElementAt(1).Selected = true;
            model.CategoryValues.ElementAt(2).Selected = true;
            model.SelectedSchema = selectedScheme;
            model.OrganisationId = organisationId;

            return model;
        }
    }
}
