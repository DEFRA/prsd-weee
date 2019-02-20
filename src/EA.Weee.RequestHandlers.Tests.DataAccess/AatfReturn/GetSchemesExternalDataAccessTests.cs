namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using System;
    using System.Collections.Generic;
    using DatabaseWrapper = Weee.Tests.Core.Model.DatabaseWrapper;
    
    public class GetSchemesExternalDataAccessTests
    {
        [Fact]
        public async void GetSchemesExternalHandlerHandleAsync_GivenGetSchemeExternalRequest_ReturnListOfSchemesThatAreApprovedOrWithdrawn()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var mapper = new SchemeMap(A.Fake<IMapper>());

                var name = "Test Name" + Guid.NewGuid();
                var tradingName = "Test Trading Name" + Guid.NewGuid();
                const string crn = "ABC12345";
                var organisationApproved = Organisation.CreateRegisteredCompany(name, crn, tradingName);
                organisationApproved.OrganisationStatus = OrganisationStatus.Complete;
                var organisationPending = Organisation.CreateRegisteredCompany(name, crn, tradingName);
                organisationPending.OrganisationStatus = OrganisationStatus.Complete;
                var organisationWithdrawn = Organisation.CreateRegisteredCompany(name, crn, tradingName);
                organisationWithdrawn.OrganisationStatus = OrganisationStatus.Complete;
                var organisationRejected = Organisation.CreateRegisteredCompany(name, crn, tradingName);
                organisationRejected.OrganisationStatus = OrganisationStatus.Complete;

                context.Organisations.Add(organisationApproved);
                context.Organisations.Add(organisationPending);
                context.Organisations.Add(organisationWithdrawn);
                context.Organisations.Add(organisationRejected);
                await context.SaveChangesAsync();

                var schemeApproved = new Scheme(organisationApproved.Id);
                schemeApproved.SetStatus(SchemeStatus.Approved);
                context.Schemes.Add(schemeApproved);
                var schemeApprovedData = mapper.Map(schemeApproved);

                var schemePending = new Scheme(organisationPending.Id);
                schemePending.SetStatus(SchemeStatus.Pending);
                context.Schemes.Add(schemePending);

                var schemeWithdrawn = new Scheme(organisationWithdrawn.Id);
                schemeWithdrawn.SetStatus(SchemeStatus.Approved);
                schemeWithdrawn.SetStatus(SchemeStatus.Withdrawn);
                context.Schemes.Add(schemeWithdrawn);
                var schemeWithdrawnData = mapper.Map(schemeWithdrawn);

                var schemeRejected = new Scheme(organisationRejected.Id);
                schemeRejected.SetStatus(SchemeStatus.Rejected);
                context.Schemes.Add(schemeRejected);

                await context.SaveChangesAsync();

                var dataAccess = new GetSchemesDataAccess(database.WeeeContext);

                var results = await dataAccess.GetCompleteSchemes();

                var handler = new GetSchemesExternalHandler(dataAccess, mapper, A.Fake<IWeeeAuthorization>());

                var request = new GetSchemesExternal();

                var schemeDataList = new List<SchemeData>();

                var result = await handler.HandleAsync(request);
            }
        }
    }
}
