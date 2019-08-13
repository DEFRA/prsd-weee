namespace EA.Weee.RequestHandlers.Tests.Unit.Aatf
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetAatfByIdExternalHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext context;
        private readonly IFetchAatfDataAccess dataAccess;
        private readonly IMap<AatfContact, AatfContactData> mapper;
        private GetAatfByIdExternalHandler handler;

        public GetAatfByIdExternalHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.context = A.Fake<IUserContext>();
            this.dataAccess = A.Fake<IFetchAatfDataAccess>();
            this.mapper = A.Fake<IMap<AatfContact, AatfContactData>>();

            this.handler = new GetAatfByIdExternalHandler(authorization, context, mapper, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new GetAatfByIdExternalHandler(authorization,
                A.Dummy<IUserContext>(),
                A.Dummy<IMap<AatfContact, AatfContactData>>(),
                A.Dummy<IFetchAatfDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfByIdExternal>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();

            await handler.HandleAsync(new GetAatfByIdExternal(aatfId, Guid.NewGuid()));

            A.CallTo(() => dataAccess.FetchById(aatfId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenAatfData_ContactDataShouldBeMapped()
        {
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => dataAccess.FetchById(A<Guid>._)).Returns(aatf);

            await handler.HandleAsync(A.Dummy<GetAatfByIdExternal>());

            A.CallTo(() => mapper.Map(aatf.Contact)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenMappedAatfData_AatfDataExternalShouldBeReturn()
        {
            var aatf = A.Fake<Aatf>();
            var aatfContactData = A.Fake<AatfContactData>();

            A.CallTo(() => dataAccess.FetchById(A<Guid>._)).Returns(aatf);
            A.CallTo(() => mapper.Map(aatf.Contact)).Returns(aatfContactData);

            var result = await handler.HandleAsync(A.Dummy<GetAatfByIdExternal>());

            result.Contact.Should().BeEquivalentTo(aatfContactData);
        }
    }
}
