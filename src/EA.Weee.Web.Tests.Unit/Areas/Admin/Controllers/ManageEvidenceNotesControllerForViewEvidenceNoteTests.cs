namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Filters;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ManageEvidenceNotesControllerForViewEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ViewEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNote", new[]
                {
                    typeof(Guid),
                    typeof(int),
                    typeof(string),
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ViewEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNote", new[]
                {
                    typeof(Guid),
                    typeof(int),
                    typeof(string),
                }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenEvidenceId_EvidenceNoteShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenRequestData_EvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = null;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(EvidenceNoteData) &&
                     v.NoteStatus == null &&
                     v.PrintableVersion == false &&
                     v.User == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task ViewEvidenceNoteGet_GivenRequestDataAndTempData_EvidenceNoteModelShouldBeBuilt(NoteStatus status)
        {
            //arrange
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = status;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(EvidenceNoteData) &&
                     v.NoteStatus.Equals(status) &&
                     v.PrintableVersion == false &&
                     v.User == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ViewEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_WhenNoteTypeIsEvidence_ModelWithRedirectTabIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId) as ViewResult;

            var model = result.Model as ViewEvidenceNoteViewModel;
            model.Type = NoteType.Evidence;

            //assert
            model.InternalUserRedirectTab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes.ToDisplayString());
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_WhenNoteTypeIsTransfer_ModelWithRedirectTabIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId) as ViewResult;

            var model = result.Model as ViewEvidenceNoteViewModel;
            model.Type = NoteType.Transfer;

            //assert
            model.InternalUserRedirectTab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_WhenPageNumberIsSetToTheViewBag_ViewBagShouldHaveThePageNumber()
        {
            // arrange 
            var page = TestFixture.Create<int>();

            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId, page);

            //assert
            ((int)ManageEvidenceController.ViewBag.Page).Should().Be(page);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_WhenPageNumberIsNotSetInViewBag_ViewBagShouldHaveDefaultPageNumber()
        {
            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            ((int)ManageEvidenceController.ViewBag.Page).Should().Be(1);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_WhenQueryStringIsNotSet_QueryStringShouldNotBeMapped()
        {
            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.QueryString == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_WhenQueryStringIsSet_QueryStringShouldBeMapped()
        {
            //arrange
            var queryString = TestFixture.Create<string>();

            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId, 1, queryString);

            //assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.QueryString == queryString))).MustHaveHappenedOnceExactly();
        }
    }
}
