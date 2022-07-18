namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.Aatf;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Security;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class CreateEvidenceNoteRequestHandlerTests : SimpleUnitTestBase
    {
        private CreateEvidenceNoteRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly CreateEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Organisation recipientOrganisation;
        private readonly Aatf aatf;
        private readonly Scheme scheme;
        private readonly Guid userId;

        public CreateEvidenceNoteRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            userContext = A.Fake<IUserContext>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            organisation = A.Fake<Organisation>();
            recipientOrganisation = A.Fake<Organisation>();
            aatf = A.Fake<Aatf>();
            scheme = A.Fake<Scheme>();
            var note = A.Fake<Note>();

            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => note.Reference).Returns(1);
            A.CallTo(() => scheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => scheme.Organisation).Returns(recipientOrganisation);
            A.CallTo(() => organisation.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => aatf.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => genericDataAccess.Add(A<Note>._)).Returns(note);

            request = new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                TestFixture.Create<WasteType>(),
                TestFixture.Create<Protocol>(),
                TestFixture.CreateMany<TonnageValues>().ToList(),
                TestFixture.Create<Core.AatfEvidence.NoteStatus>(),
                Guid.Empty);

            handler = new CreateEvidenceNoteRequestHandler(weeeAuthorization,
                genericDataAccess,
                aatfDataAccess,
                userContext,
                systemDataDataAccess);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => aatfDataAccess.GetDetails(aatf.Id)).Returns(aatf);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => genericDataAccess.GetById<Scheme>(request.RecipientId)).Returns(scheme);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new CreateEvidenceNoteRequestHandler(authorization, genericDataAccess,
                aatfDataAccess,
                userContext,
                systemDataDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new CreateEvidenceNoteRequestHandler(authorization, genericDataAccess,
                aatfDataAccess,
                userContext,
                systemDataDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoOrganisationFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Organisation>(A<Guid>._)).Returns((Organisation)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoSchemeFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Scheme>(A<Guid>._)).Returns((Scheme)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfDoesNotBelongToTheOrganisation_ShouldThrowInvalidOperationException()
        {
            //arrange
            A.CallTo(() => aatfDataAccess.GetDetails(aatf.Id)).Returns(A.Fake<Aatf>());

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldGetSystemDateTime()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldEnsureCanAccessExternalArea()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_AatfForComplianceYearShouldBeRetrieved()
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();
            A.CallTo(() => aatf.AatfId).Returns(aatfId);
            A.CallTo(() => aatfDataAccess.GetDetails(aatf.Id)).Returns(aatf);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => aatfDataAccess.GetAatfByAatfIdAndComplianceYear(aatfId, request.StartDate.Year))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndThereIsNoAatfForTheComplianceYear_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => aatfDataAccess.GetDetails(A<Guid>._)).Returns(aatf);
            A.CallTo(() => aatfDataAccess.GetAatfByAatfIdAndComplianceYear(A<Guid>._, A<int>._)).Returns((Aatf)null);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenDraftRequest_NoteShouldBeAddedToContext()
        {
            //act
            var currentDate = TestFixture.Create<DateTime>();
            SystemTime.Freeze(currentDate);

            A.CallTo(() => aatfDataAccess.GetAatfByAatfIdAndComplianceYear(A<Guid>._, A<int>._)).Returns(aatf);

            //arrange
            var request = new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                TestFixture.Create<WasteType>(),
                TestFixture.Create<Protocol>(),
                TestFixture.CreateMany<TonnageValues>().ToList(),
                Core.AatfEvidence.NoteStatus.Draft,
                Guid.Empty);

            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.EndDate.Equals(request.EndDate) &&
                                                                           n.Aatf.Equals(aatf) &&
                                                                           n.CreatedDate == currentDate &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol.ToInt().Equals(request.Protocol.ToInt()) &&
                                                                           n.WasteType.ToInt().Equals(request.WasteType.ToInt()) &&
                                                                           n.Recipient.Equals(recipientOrganisation) &&
                                                                           n.StartDate.Equals(request.StartDate) &&
                                                                           n.EndDate.Equals(request.EndDate) &&
                                                                           n.CreatedById.Equals(userId.ToString()) &&
                                                                           n.NoteType.Equals(NoteType.EvidenceNote) &&
                                                                           n.Status.Equals(NoteStatus.Draft) &&
                                                                           n.NoteTonnage.Count.Equals(request.TonnageValues.Count) &&
                                                                           n.NoteStatusHistory.Count.Equals(0))))
                                                                           .MustHaveHappenedOnceExactly();

            foreach (var requestTonnageValue in request.TonnageValues)
            {
                A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteTonnage.FirstOrDefault(c =>
                    c.CategoryId.Equals((WeeeCategory)requestTonnageValue.CategoryId)
                    && c.Reused.Equals(requestTonnageValue.SecondTonnage)
                    && c.Received.Equals(requestTonnageValue.FirstTonnage)) != null))).MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenSubmittedRequest_NoteShouldBeAddedToContext()
        {
            //act
            var currentDate = TestFixture.Create<DateTime>();
            SystemTime.Freeze(currentDate);

            var systemDateTime = TestFixture.Create<DateTime>();
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(systemDateTime);

            A.CallTo(() => aatfDataAccess.GetAatfByAatfIdAndComplianceYear(A<Guid>._, A<int>._)).Returns(aatf);

            //arrange
            var request = new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                TestFixture.Create<WasteType>(),
                TestFixture.Create<Protocol>(),
                TestFixture.CreateMany<TonnageValues>().ToList(),
                Core.AatfEvidence.NoteStatus.Submitted,
                Guid.Empty);

            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.EndDate.Equals(request.EndDate) &&
                                                                           n.Aatf.Equals(aatf) &&
                                                                           n.CreatedDate.Equals(currentDate) &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol.ToInt().Equals(request.Protocol.ToInt()) &&
                                                                           n.WasteType.ToInt().Equals(request.WasteType.ToInt()) &&
                                                                           n.Recipient.Equals(recipientOrganisation) &&
                                                                           n.StartDate.Equals(request.StartDate) &&
                                                                           n.EndDate.Equals(request.EndDate) &&
                                                                           n.CreatedById.Equals(userId.ToString()) &&
                                                                           n.NoteType.Equals(NoteType.EvidenceNote) &&
                                                                           n.Status.Equals(NoteStatus.Submitted) &&
                                                                           n.NoteTonnage.Count.Equals(request.TonnageValues.Count) &&
                                                                           n.NoteStatusHistory.Count.Equals(1))))
                                                                           .MustHaveHappenedOnceExactly();

            foreach (var requestTonnageValue in request.TonnageValues)
            {
                A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteTonnage.FirstOrDefault(c =>
                    c.CategoryId.Equals((WeeeCategory)requestTonnageValue.CategoryId)
                    && c.Reused.Equals(requestTonnageValue.SecondTonnage)
                    && c.Received.Equals(requestTonnageValue.FirstTonnage)) != null))).MustHaveHappenedOnceExactly();
            }

            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteStatusHistory.First(c =>
                c.ChangedById.Equals(userId.ToString()) &&
                c.ChangedDate.Year == systemDateTime.Year &&
                c.ChangedDate.Month == currentDate.Month &&
                c.ChangedDate.Day == currentDate.Day &&
                c.FromStatus.Equals(NoteStatus.Draft) &&
                c.ToStatus.Equals(NoteStatus.Submitted)) != null))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithNullWasteAndProtocol_NoteShouldBeAddedToContext()
        {
            //act
            var currentDate = TestFixture.Create<DateTime>();
            SystemTime.Freeze(currentDate);

            A.CallTo(() => aatfDataAccess.GetAatfByAatfIdAndComplianceYear(A<Guid>._, A<int>._)).Returns(aatf);

            var newRequest = new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                SystemTime.Now,
                SystemTime.Now.AddDays(1),
                null,
                null,
                new List<TonnageValues>(),
                Core.AatfEvidence.NoteStatus.Draft,
                Guid.Empty);

            //arrange
            await handler.HandleAsync(newRequest);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.EndDate.Equals(newRequest.EndDate) &&

                                                                           n.Aatf.Equals(aatf) &&
                                                                           n.CreatedDate.Equals(currentDate) &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol.Equals(null) &&
                                                                           n.WasteType.Equals(null) &&
                                                                           n.Recipient.Equals(recipientOrganisation) &&
                                                                           n.StartDate.Equals(newRequest.StartDate) &&
                                                                           n.EndDate.Equals(newRequest.EndDate) &&
                                                                           n.CreatedById.Equals(userId.ToString()) &&
                                                                           n.NoteType.Equals(NoteType.EvidenceNote) &&
                                                                           n.Status.Equals(NoteStatus.Draft) &&
                                                                           n.NoteTonnage.Count.Equals(newRequest.TonnageValues.Count) &&
                                                                           n.NoteStatusHistory.Count.Equals(0))))
                                                                           .MustHaveHappenedOnceExactly();
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ReferenceShouldBeReturned()
        {
            //act
            var id = TestFixture.Create<Guid>();
            var newNote = A.Fake<Note>();
            A.CallTo(() => newNote.Id).Returns(id);
            A.CallTo(() => genericDataAccess.Add(A<Note>._)).Returns(newNote);

            //arrange
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(id);
        }

        private CreateEvidenceNoteRequest Request()
        {
            return new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                TestFixture.Create<WasteType>(),
                TestFixture.Create<Protocol>(),
                new List<TonnageValues>(),
                TestFixture.Create<Core.AatfEvidence.NoteStatus>(),
                Guid.Empty);
        }
    }
}
