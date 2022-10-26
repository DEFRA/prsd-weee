namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using AutoFixture;
    using Constant;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNotesViewModelTests : SimpleUnitTestBase
    {
        private readonly ISessionService service;

        private readonly TransferEvidenceNotesViewModel model;

        public TransferEvidenceNotesViewModelTests()
        {
            service = A.Fake<ISessionService>();

            model = new TransferEvidenceNotesViewModel()
            {
                SessionService = service
            };
        }

        [Fact]
        public void TransferEvidenceNotesViewModel_ShouldImplement_IValidatableObject()
        {
            typeof(TransferEvidenceNotesViewModel).Should().Implement<IValidatableObject>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModel_Constructor_ShouldInitialiseLists()
        {
            //assert
            model.EvidenceNotesDataList.Should().NotBeNull();
            model.CategoryValues.Should().NotBeNull();
        }

        [Theory]
        [InlineData("outgoingTransferKey", true)]
        [InlineData("transferNote", false)]
        public void Validate_GivenNoNotesSelected_ValidationResultExpected(string sessionKey, bool isEdit)
        {
            //arrange
            model.IsEdit = isEdit;
            var transferEvidenceNoteRequest = new TransferEvidenceNoteRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(), TestFixture.CreateMany<int>().ToList(), new List<Guid>(), new List<Guid>(), TestFixture.Create<bool>());

            A.CallTo(() => service.GetTransferSessionObject<TransferEvidenceNoteRequest>(sessionKey))
                .Returns(transferEvidenceNoteRequest);

            //act
            var validationResults = model.Validate(new ValidationContext(model));

            //assert
            validationResults.Should().Contain(v =>
                v.ErrorMessage.Equals("Select at least one evidence note to transfer from")
                && v.MemberNames.Contains(ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError));
        }

        [Theory]
        [InlineData("outgoingTransferKey", true)]
        [InlineData("transferNote", false)]
        public void Validate_GivenMoreThanFiveNotesSelected_ValidationResultExpected(string sessionKey, bool isEdit)
        {
            model.IsEdit = isEdit;
            var transferEvidenceNoteRequest = new TransferEvidenceNoteRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(), TestFixture.CreateMany<int>().ToList(), TestFixture.CreateMany<Guid>(6).ToList(), new List<Guid>(), TestFixture.Create<bool>());

            A.CallTo(() => service.GetTransferSessionObject<TransferEvidenceNoteRequest>(sessionKey))
                .Returns(transferEvidenceNoteRequest);

            //act
            var validationResults = model.Validate(new ValidationContext(model));

            //assert
            validationResults.Should().Contain(v =>
                v.ErrorMessage.Equals("You cannot select more than 5 notes")
                && v.MemberNames.Contains(ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError));
        }

        [Theory]
        [InlineData("outgoingTransferKey", true)]
        [InlineData("transferNote", false)]
        public void Validate_GivenFivesNotesSelected_ValidationResultShouldBeEmpty(string sessionKey, bool isEdit)
        {
            model.IsEdit = isEdit;
            var transferEvidenceNoteRequest = new TransferEvidenceNoteRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(), TestFixture.CreateMany<int>().ToList(), TestFixture.CreateMany<Guid>(5).ToList(), new List<Guid>(), TestFixture.Create<bool>());

            A.CallTo(() => service.GetTransferSessionObject<TransferEvidenceNoteRequest>(sessionKey))
                .Returns(transferEvidenceNoteRequest);

            //act
            var validationResults = model.Validate(new ValidationContext(model));

            //assert
            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("outgoingTransferKey", true)]
        [InlineData("transferNote", false)]
        public void Validate_GivenSingleNoteSelected_ValidationResultShouldBeEmpty(string sessionKey, bool isEdit)
        {
            model.IsEdit = isEdit;
            var transferEvidenceNoteRequest = new TransferEvidenceNoteRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<Guid>(), TestFixture.CreateMany<int>().ToList(), TestFixture.CreateMany<Guid>(1).ToList(), new List<Guid>(), TestFixture.Create<bool>());

            A.CallTo(() => service.GetTransferSessionObject<TransferEvidenceNoteRequest>(sessionKey))
                .Returns(transferEvidenceNoteRequest);

            //act
            var validationResults = model.Validate(new ValidationContext(model));

            //assert
            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void SearchPerformed_GivenNoSearchRef_ShouldReturnFalse(string search)
        {
            //arrange
            var model = new TransferEvidenceNotesViewModel() { SearchRef = search };

            //act
            var result = model.SearchPerformed;

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void SearchPerformed_GivenSearchRef_ShouldReturnTrue()
        {
            //arrange
            var model = new TransferEvidenceNotesViewModel() { SearchRef = "search" };

            //act
            var result = model.SearchPerformed;

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void SearchRef_ShouldHaveDisplayAttribute()
        {
            typeof(TransferEvidenceNotesViewModel).GetProperty("SearchRef").Should().BeDecoratedWith<DisplayAttribute>().Which.Name.Should().Be("Search by reference ID");
        }
    }
}
