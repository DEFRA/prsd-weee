namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Requests
{
    using System;
    using System.Linq;
    using AutoFixture;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Scheme.Requests;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class CreateTransferEvidenceNoteRequestCreatorTests
    {
        private readonly CreateTransferEvidenceNoteRequestCreator requestCreator;
        private readonly Fixture fixture;

        public CreateTransferEvidenceNoteRequestCreatorTests()
        {
            fixture = new Fixture();

            requestCreator = new CreateTransferEvidenceNoteRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenNullViewModel_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenSelectedSchemeIsNull_InvalidOperationExceptionExpected()
        {
            //arrange
            var model = GetModel();
            model.SelectedSchema = null;

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenThereIsNoCategorySelected_InvalidOperationExceptionExpected()
        {
            //arrange
            var model = GetModel();
           
            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidModel_CreateTransferNoteRequestShoulBeCreated()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var selectedScheme = Guid.NewGuid();
            var schemes = fixture.CreateMany<SchemeData>().ToList();

            var model = GetValidModelWithSelectedCategories(selectedScheme);
            var selectedIds = model.CategoryValues.Where(x => x.Selected).Select(x => x.CategoryId).ToList();

           //act
           var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.CategoryIds.Should().BeEquivalentTo(selectedIds);
            request.SchemeId.Should().Be(selectedScheme);
        }

        private TransferEvidenceNoteDataViewModel GetModel()
        {
            return new TransferEvidenceNoteDataViewModel();
        }

        private TransferEvidenceNoteDataViewModel GetValidModelWithSelectedCategories(Guid selectedScheme)
        {
            var model = GetModel();
            model.CategoryValues.ElementAt(0).Selected = true;
            model.CategoryValues.ElementAt(1).Selected = true;
            model.CategoryValues.ElementAt(2).Selected = true;
            model.SelectedSchema = selectedScheme;

            return model;
        }
    }
}
