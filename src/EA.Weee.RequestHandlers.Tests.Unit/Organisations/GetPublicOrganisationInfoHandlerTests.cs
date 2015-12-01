namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;

    public class GetPublicOrganisationInfoHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        [Fact]
        public async Task GetPublicOrganisationInfoHandler_OrganisationNotAvailable_ThrowsArgumentException()
        {
            var organisationId = Guid.NewGuid();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new GetPublicOrganisationInfoHandler(context, A<PublicOrganisationMap>._);
            var message = new GetPublicOrganisationInfo(organisationId);

            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task GetPublicOrganisationInfoHandler_HappyPath_ReturnsPublicOrganisationInfo()
        {
            var organisationId = Guid.NewGuid();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            var expectedReturnValue = new PublicOrganisationData();
            Organisation mappedOrganisation = null;
            var publicOrganisationMap = A.Fake<IMap<Organisation, PublicOrganisationData>>();
            
            A.CallTo(() => publicOrganisationMap.Map(A<Organisation>._))
                .Invokes((Organisation o) => mappedOrganisation = o)
                .Returns(expectedReturnValue);

            var handler = new GetPublicOrganisationInfoHandler(context, publicOrganisationMap);
            var message = new GetPublicOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.NotNull(mappedOrganisation);
            Assert.Equal(organisationId, mappedOrganisation.Id);
            Assert.Same(expectedReturnValue, result);
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            dbHelper.SetId(organisation, id);
            return organisation;
        }
    }
}
