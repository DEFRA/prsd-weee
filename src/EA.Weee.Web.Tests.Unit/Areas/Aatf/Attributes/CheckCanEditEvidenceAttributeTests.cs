﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Tests.Unit.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Services.Caching;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.Helpers;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.AatfReturn;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class CheckCanEditEvidenceAttributeTests : SimpleUnitTestBase
    {
        private readonly CheckCanEditEvidenceNoteAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;
        private readonly IWeeeCache cache;

        public CheckCanEditEvidenceAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            aatfEvidenceHelper = A.Fake<IAatfEvidenceHelper>();
            cache = A.Fake<IWeeeCache>();

            attribute = new CheckCanEditEvidenceNoteAttribute { Client = () => client, AatfEvidenceHelper = aatfEvidenceHelper, Cache = cache };
            context = A.Fake<ActionExecutingContext>();

            var routeData = new RouteData();
            routeData.Values.Add("evidenceNoteId", Guid.NewGuid());
            A.CallTo(() => context.RouteData).Returns(routeData);
        }

        [Fact]
        public void OnActionExecuting_GivenViewModel_AndNotEvidenceNoteViewModel_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "viewModel", new object() } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("Edit evidence note view model incorrect type.");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenViewModel_AndEvidenceNoteIsNotDraftOrReturned_InvalidOperationExceptionExpected(NoteStatus noteStatus)
        {
            if (noteStatus != NoteStatus.Draft && noteStatus != NoteStatus.Returned)
            {
                //arrange
                var viewModel = new EvidenceNoteViewModel()
                {
                    Id = Guid.NewGuid()
                };

                var actionParameters = new Dictionary<string, object> { { "viewModel", viewModel } };
                A.CallTo(() => context.ActionParameters).Returns(actionParameters);

                var note = TestFixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;

                A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals(viewModel.Id)))).Returns(note);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public void OnActionExecuting_GivenNoEvidenceNoteId_ArgumentExceptionExpected()
        {
            //arrange
            A.CallTo(() => context.RouteData).Returns(new RouteData());

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("No evidence note ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsNotGuid_ArgumentExceptionExpected()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("evidenceNoteId", 1);

            A.CallTo(() => context.RouteData).Returns(routeData);
            
            //act
            Action action = () => attribute.OnActionExecuting(context);
            
            //assert
            action.Should().Throw<ArgumentException>().WithMessage("The specified evidence note ID is not valid.");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenEvidenceNoteIdIsNotDraft_InvalidOperationExceptionExpected(NoteStatus noteStatus)
        {
            if (noteStatus != NoteStatus.Draft && noteStatus != NoteStatus.Returned)
            {
                //arrange
                var note = TestFixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;

                A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()                    
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsDraft_NoExceptionExpected()
        {
            //arrange
            var note = TestFixture.Create<EvidenceNoteData>();
            note.Status = NoteStatus.Draft;

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(A<List<AatfData>>._, A<Guid>._, A<int>._)).Returns(true);

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);
            //act
            var result = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert

            result.Should().BeNull();
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsReturned_NoExceptionExpected()
        {
            //arrange
            var note = TestFixture.Create<EvidenceNoteData>();
            note.Status = NoteStatus.Returned;

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(A<List<AatfData>>._, A<Guid>._, A<int>._)).Returns(true);

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);

            //act
            var result = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            result.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenEvidenceNoteIdIsDraftOrReturnedAndEvidenceCanNotBeEdited_ExceptionShouldBeThrown(NoteStatus noteStatus)
        {
            if (noteStatus == NoteStatus.Draft || noteStatus == NoteStatus.Returned)
            {
                //arrange
                var note = TestFixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;
                var aatfs = TestFixture.CreateMany<AatfData>().ToList();

                A.CallTo(() => client.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(note);

                A.CallTo(() => cache.FetchAatfDataForOrganisationData(note.AatfData.Organisation.Id)).Returns(aatfs);
                A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(aatfs, note.AatfData.Id, note.ComplianceYear))
                    .Returns(false);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} cannot edit notes");
            }
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenViewModelAndNoteIsDraftOrReturnedAndEvidenceCanNotBeEdited_ExceptionShouldBeThrown(NoteStatus noteStatus)
        {
            if (noteStatus == NoteStatus.Draft || noteStatus == NoteStatus.Returned)
            {
                //arrange
                var viewModel = new EvidenceNoteViewModel()
                {
                    Id = Guid.NewGuid()
                };

                var actionParameters = new Dictionary<string, object> { { "viewModel", viewModel } };
                A.CallTo(() => context.ActionParameters).Returns(actionParameters);

                var note = TestFixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;
                var aatfs = TestFixture.CreateMany<AatfData>().ToList();

                A.CallTo(() => client.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(note);

                A.CallTo(() => client.SendAsync(A<string>._,
                        A<GetAatfByOrganisation>.That.Matches(r => r.OrganisationId == note.AatfData.Organisation.Id)))
                    .Returns(aatfs);

                A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(aatfs, note.AatfData.Id, note.ComplianceYear))
                    .Returns(false);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} cannot edit notes");
            }
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenEvidenceNoteIdIsDraftOrReturnedAndEvidenceCanBeEdited_NoExceptionShouldBeThrown(NoteStatus noteStatus)
        {
            if (noteStatus == NoteStatus.Draft || noteStatus == NoteStatus.Returned)
            {
                //arrange
                var note = TestFixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;
                var aatfs = TestFixture.CreateMany<AatfData>().ToList();

                A.CallTo(() => client.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(note);
                A.CallTo(() => cache.FetchAatfDataForOrganisationData(note.AatfData.Organisation.Id)).Returns(aatfs);
                A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(aatfs, note.AatfData.Id, note.ComplianceYear))
                    .Returns(true);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeNull();
            }
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenViewModelAndNoteIsDraftOrReturnedAndEvidenceCanBeEdited_NoExceptionShouldBeThrown(NoteStatus noteStatus)
        {
            if (noteStatus == NoteStatus.Draft || noteStatus == NoteStatus.Returned)
            {
                //arrange
                var note = TestFixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;
                var aatfs = TestFixture.CreateMany<AatfData>().ToList();

                A.CallTo(() => client.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(note);
                A.CallTo(() => cache.FetchAatfDataForOrganisationData(note.AatfData.Organisation.Id)).Returns(aatfs);
                A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(aatfs, note.AatfData.Id, note.ComplianceYear))
                    .Returns(true);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeNull();
            }
        }
    }
}
