﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Tests.Unit.Helpers;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Factories;
    using RequestHandlers.Security;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class EditEvidenceNoteRequestHandlerTests : SimpleUnitTestBase
    {
        private EditEvidenceNoteRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly EditEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Organisation recipientOrganisation;
        private readonly Scheme recipientScheme;
        private readonly Note note;
        private readonly Aatf aatf;

        public EditEvidenceNoteRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            var currentDate = new DateTime(2021, 12, 1);
            genericDataAccess = A.Fake<IGenericDataAccess>();

            recipientScheme = A.Fake<Scheme>();
            organisation = A.Fake<Organisation>();
            recipientOrganisation = A.Fake<Organisation>();
            A.CallTo(() => recipientOrganisation.Schemes).Returns(new List<Scheme>() { recipientScheme });
            A.CallTo(() => recipientScheme.Organisation).Returns(recipientOrganisation);
            note = A.Fake<Note>();
            TestFixture.Create<Guid>();
            var organisationId = TestFixture.Create<Guid>();
            aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.ApprovalDate).Returns(currentDate.AddDays(-1));
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Approved);
            A.CallTo(() => aatf.ComplianceYear).Returns((short)currentDate.Year);

            A.CallTo(() => recipientOrganisation.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => note.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => note.Status).Returns(NoteStatus.Draft);
            A.CallTo(() => note.Aatf).Returns(aatf);

            request = Request();

            handler = new EditEvidenceNoteRequestHandler(weeeAuthorization, evidenceDataAccess, schemeDataAccess, systemDataDataAccess, genericDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(request.Id)).Returns(note);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new EditEvidenceNoteRequestHandler(authorization, evidenceDataAccess, schemeDataAccess, systemDataDataAccess, genericDataAccess);

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

            handler = new EditEvidenceNoteRequestHandler(authorization, evidenceDataAccess, schemeDataAccess, systemDataDataAccess, genericDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task HandleAsync_GivenNoteIsNotInDraftAndReturned_ShouldInvalidOperationException(NoteStatus status)
        {
            //arrange
            var allowedStatus = new List<int>() { NoteStatus.Draft.Value, NoteStatus.Returned.Value };

            if (!allowedStatus.Contains(status.Value))
            {
                A.CallTo(() => note.Status).Returns(status);
                A.CallTo(() => note.RecipientId).Returns(recipientOrganisation.Id);
                A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

                //act
                var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

                //assert
                result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public async Task HandleAsync_EnsureTheSchemeNotChanged_GivenNoteStatusIsReturnedAndSchemeChanged_ShouldThrowInvalidOperationException()
        {
            //arrange 
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.RecipientId).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be($"Evidence note {note.Id} has incorrect Recipient Id to be saved");
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoSchemeFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns((Scheme)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(note.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldGetSystemDateTime()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(ProtocolData))]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled(Domain.Evidence.Protocol protocol)
        {
            //arrange
            var currentDate = new DateTime(2021, 12, 1);
            SystemTime.Freeze(currentDate);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(recipientScheme);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = Request();
            request.Protocol = (Protocol?)protocol;

            var tonnageValues = request.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.Update(note, recipientOrganisation, request.StartDate, request.EndDate, A<Domain.Evidence.WasteType>._, protocol, A<IList<NoteTonnage>>._, A<NoteStatus>._, A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)))).MustHaveHappenedOnceExactly();

            AssertTonnages(tonnageValues);
            SystemTime.Unfreeze();
        }

        [Theory]
        [ClassData(typeof(WasteTypeData))]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled(Domain.Evidence.WasteType waste)
        {
            //arrange
            var currentDate = new DateTime(2021, 12, 1);
            SystemTime.Freeze(currentDate);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(recipientScheme);
           
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = Request();
            request.WasteType = (WasteType?)waste;

            var tonnageValues = request.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.Update(note, recipientOrganisation, request.StartDate, request.EndDate, waste, A<Domain.Evidence.Protocol>._, A<IList<NoteTonnage>>._, A<NoteStatus>._,
                A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)))).MustHaveHappenedOnceExactly();

            AssertTonnages(tonnageValues);

            SystemTime.Unfreeze();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled(Domain.Evidence.NoteStatus status)
        {
            //arrange
            var currentDate = new DateTime(2021, 12, 1);
            SystemTime.Freeze(currentDate);
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(recipientScheme);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = Request();
            request.Status = (Core.AatfEvidence.NoteStatus)status.Value;

            var tonnageValues = request.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.Update(note, recipientOrganisation, request.StartDate, request.EndDate, A<Domain.Evidence.WasteType>._, A<Domain.Evidence.Protocol>._, A<IList<NoteTonnage>>._, status, A<DateTime>.That.IsEqualTo(CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate)))).MustHaveHappenedOnceExactly();

            AssertTonnages(tonnageValues);
            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_IdShouldBeReturned()
        {
            //act
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(recipientScheme);

            //arrange
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(note.Id);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndReturnedStatus_IdShouldBeReturned()
        {
            //act
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.RecipientId).Returns(recipientOrganisation.Id);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(recipientScheme);

            //arrange
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(note.Id);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWhereAatfStatusIsCancelled_InvalidOperationExceptionExpected()
        {
            //arrange
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Cancelled);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Contain("is in an invalid state to be saved");
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWhereAatfStatusIsSuspended_InvalidOperationExceptionExpected()
        {
            //arrange
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Suspended);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Contain("is in an invalid state to be saved");
        }

        public static IEnumerable<object[]> OutOfComplianceYear =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 2, 1), 2019 },
                new object[] { new DateTime(2020, 1, 1), 2022 },
            };

        [Theory]
        [MemberData(nameof(OutOfComplianceYear))]
        public async Task HandleAsync_GivenRequestWhereComplianceYearInvalid_InvalidOperationExceptionExpected(DateTime systemDateTime, int complianceYear)
        {
            //arrange
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(systemDateTime);
            A.CallTo(() => aatf.ComplianceYear).Returns((short)complianceYear);
            A.CallTo(() => aatf.ApprovalDate).Returns(systemDateTime.AddDays(1));

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Contain("is in an invalid state to be saved");
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWhereAatfApprovalDateIsInvalid_InvalidOperationExceptionExpected()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);
            A.CallTo(() => aatf.ApprovalDate).Returns(currentDate.AddDays(1));
            A.CallTo(() => aatf.ComplianceYear).Returns((short)currentDate.Year);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Contain("is in an invalid state to be saved");
        }

        private void AssertTonnages(List<NoteTonnage> tonnageValues)
        {
            foreach (var tonnageValue in tonnageValues)
            {
                A.CallTo(() => evidenceDataAccess.Update(A<Note>._,
                        A<Organisation>._, A<DateTime>._, A<DateTime>._, A<Domain.Evidence.WasteType>._, A<Domain.Evidence.Protocol>._,
                        A<IList<NoteTonnage>>.That.Matches(t =>
                            t.Count(n => n.CategoryId.Equals(tonnageValue.CategoryId) &&
                                         n.Received.Equals(tonnageValue.Received) &&
                                         n.Reused.Equals(tonnageValue.Reused)).Equals(1)), A<NoteStatus>._, A<DateTime>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        private EditEvidenceNoteRequest Request()
        {
            return new EditEvidenceNoteRequest(organisation.Id,
                TestFixture.Create<Guid>(),
                recipientOrganisation.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                TestFixture.Create<WasteType>(),
                TestFixture.Create<Protocol>(),
                TestFixture.CreateMany<TonnageValues>().ToList(),
                TestFixture.Create<Core.AatfEvidence.NoteStatus>(),
                TestFixture.Create<Guid>());
        }
    }
}
