namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Security;
    using Requests.Aatf;
    using Requests.AatfEvidence;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class CreateEvidenceNoteRequestHandlerTests
    {
        private CreateEvidenceNoteRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;
        private readonly CreateEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Aatf aatf;
        private readonly Scheme scheme;
        private readonly Note note;

        public CreateEvidenceNoteRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            userContext = A.Fake<IUserContext>();

            organisation = A.Fake<Organisation>();
            aatf = A.Fake<Aatf>();
            scheme = A.Fake<Scheme>();
            note = A.Fake<Note>();

            A.CallTo(() => note.Reference).Returns(1);
            A.CallTo(() => scheme.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => aatf.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => genericDataAccess.Add<Note>(A<Note>._)).Invokes(() =>
            {
                A.CallTo(() => note.Reference).Returns(100);
            });

            request = new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                fixture.Create<WasteType>(),
                fixture.Create<Protocol>(),
                new List<TonnageValues>());

            handler = new CreateEvidenceNoteRequestHandler(weeeAuthorization,
                genericDataAccess,
                aatfDataAccess,
                userContext);

            A.CallTo(() => genericDataAccess.GetById<Organisation>(request.OrganisationId)).Returns(organisation);
            A.CallTo(() => aatfDataAccess.GetDetails(aatf.Id)).Returns(aatf);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new CreateEvidenceNoteRequestHandler(authorization, genericDataAccess,
                aatfDataAccess,
                userContext);

            //act
            Func<Task> action = async () => await handler.HandleAsync(Request());

            //assert
            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalOrOrganisationAccess().Build();

            handler = new CreateEvidenceNoteRequestHandler(authorization, genericDataAccess,
                aatfDataAccess,
                userContext);

            //act
            Func<Task> action = async () => await handler.HandleAsync(Request());

            //assert
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public void HandleAsync_GivenRequestAndNoOrganisationFound_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Organisation>(A<Guid>._)).Returns((Organisation)null);

            //act
            Func<Task> action = async () => await handler.HandleAsync(Request());

            //assert
            action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void HandleAsync_GivenRequestAndNoSchemeFound_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<Scheme>(A<Guid>._)).Returns((Scheme)null);

            //act
            Func<Task> action = async () => await handler.HandleAsync(Request());

            //assert
            action.Should().ThrowAsync<ArgumentNullException>();
        }

        //[Fact]
        //public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        //{
        //    //act
        //    await handler.HandleAsync(request);

        //    //assert
        //    A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId))
        //        .MustHaveHappenedOnceExactly();
        //}

        private CreateEvidenceNoteRequest Request()
        {
            return new CreateEvidenceNoteRequest(organisation.Id,
                aatf.Id,
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                fixture.Create<WasteType>(),
                fixture.Create<Protocol>(),
                new List<TonnageValues>());
        }
    }
}
