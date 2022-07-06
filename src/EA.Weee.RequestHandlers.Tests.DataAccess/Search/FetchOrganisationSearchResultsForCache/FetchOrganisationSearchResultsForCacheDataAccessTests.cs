namespace EA.Weee.RequestHandlers.Tests.DataAccess.Search.FetchOrganisationSearchResultsForCache
{
    using AutoFixture;
    using EA.Weee.Core.Search;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using FluentAssertions;
    using Xunit;
    using Organisation = Weee.Tests.Core.Model.Organisation;

    public class FetchOrganisationSearchResultsForCacheDataAccessTests
    {
        private readonly Fixture fixture;
        private readonly WeeeContext context;

        public FetchOrganisationSearchResultsForCacheDataAccessTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
        }

        /// <summary>
        /// Ensures that the data access does not return incomplete organisations.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCompleteOrganisations_WithIncompleteOrganisation_OrganisationNotReturned()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = new Organisation();
                organisation.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation.TradingName = "Incomplete test organisation";
                organisation.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.SoleTraderOrIndividual.Value;
                organisation.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Incomplete.Value;

                Scheme scheme = new Scheme();
                scheme.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                scheme.Organisation = organisation;
                scheme.SchemeStatus = (int)Core.Shared.SchemeStatus.Pending;

                database.Model.Organisations.Add(organisation);
                database.Model.Schemes.Add(scheme);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.DoesNotContain(results,
                    r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4"));
            }
        }

        /// <summary>
        /// Ensures that the data access does not return organisations representing
        /// schemes with a status of rejected.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCompleteOrganisations_WithRejectedScheme_OrganisationNotReturned()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = new Organisation();
                organisation.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation.TradingName = "Test organisation";
                organisation.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.SoleTraderOrIndividual.Value;
                organisation.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme = new Scheme();
                scheme.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                scheme.Organisation = organisation;
                scheme.SchemeStatus = (int)Core.Shared.SchemeStatus.Rejected;

                database.Model.Organisations.Add(organisation);
                database.Model.Schemes.Add(scheme);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.DoesNotContain(results,
                    r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4"));
            }
        }

        /// <summary>
        /// Ensures that search results representing organisations which are companies will use
        /// the 'Name' column from the database as the organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCompleteOrganisations_WithCompleteCompany_UsesNameColumnToPopulateOrganisationName()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = new Organisation();
                organisation.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation.Name = "Company Name";
                organisation.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.RegisteredCompany.Value;
                organisation.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme = new Scheme();
                scheme.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                scheme.Organisation = organisation;
                scheme.SchemeStatus = (int)Core.Shared.SchemeStatus.Approved;

                database.Model.Organisations.Add(organisation);
                database.Model.Schemes.Add(scheme);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.Contains(results,
                    r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4") && r.Name == "Company Name");
            }
        }

        /// <summary>
        /// Ensures that search results representing organisations which are sole traders or individuals will use
        /// the 'Name' column from the database as the organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCompleteOrganisations_WithCompleteSoleTraderOrIndividual_UsesTradingNameColumnToPopulateOrganisationName()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = new Organisation();
                organisation.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation.Name = "Name";
                organisation.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.SoleTraderOrIndividual.Value;
                organisation.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme = new Scheme();
                scheme.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                scheme.Organisation = organisation;
                scheme.SchemeStatus = (int)Core.Shared.SchemeStatus.Approved;

                database.Model.Organisations.Add(organisation);
                database.Model.Schemes.Add(scheme);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.Contains(results,
                    r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4") && r.Name == "Name");
            }
        }

        /// <summary>
        /// Ensures that search results representing organisations which are partnerships will use
        /// the 'TradingName' column from the database as the organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCompleteOrganisations_WithCompletePartnership_UsesTradingNameColumnToPopulateOrganisationName()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = new Organisation();
                organisation.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation.TradingName = "Trading Name";
                organisation.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.Partnership.Value;
                organisation.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme = new Scheme();
                scheme.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                scheme.Organisation = organisation;
                scheme.SchemeStatus = (int)Core.Shared.SchemeStatus.Approved;

                database.Model.Organisations.Add(organisation);
                database.Model.Schemes.Add(scheme);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.Contains(results,
                    r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4") && r.Name == "Trading Name");
            }
        }

        /// <summary>
        /// Ensures that search results are ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCompleteOrganisations_WithSeveralResults_ReturnsResultsOrderedByName()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation1 = new Organisation();
                organisation1.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation1.Name = "Company B";
                organisation1.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.RegisteredCompany.Value;
                organisation1.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme1 = new Scheme();
                scheme1.Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B");
                scheme1.Organisation = organisation1;
                scheme1.SchemeStatus = (int)Core.Shared.SchemeStatus.Approved;

                Organisation organisation2 = new Organisation();
                organisation2.Id = new Guid("659A5E1B-90F8-4E5C-8939-436189424AB6");
                organisation2.Name = "Company A";
                organisation2.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.RegisteredCompany.Value;
                organisation2.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme2 = new Scheme();
                scheme2.Id = new Guid("0F0CCEEF-849B-474A-A548-C52F99FD0C99");
                scheme2.Organisation = organisation2;
                scheme2.SchemeStatus = (int)Core.Shared.SchemeStatus.Approved;

                Organisation organisation3 = new Organisation();
                organisation3.Id = new Guid("D7C37279-C3F5-44C0-B6CF-D43A968F3F29");
                organisation3.Name = "Company C";
                organisation3.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.RegisteredCompany.Value;
                organisation3.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                Scheme scheme3 = new Scheme();
                scheme3.Id = new Guid("52DA530C-B09C-4C52-B354-F07E72EE6111");
                scheme3.Organisation = organisation3;
                scheme3.SchemeStatus = (int)Core.Shared.SchemeStatus.Approved;

                database.Model.Organisations.Add(organisation1);
                database.Model.Organisations.Add(organisation2);
                database.Model.Organisations.Add(organisation3);
                database.Model.Schemes.Add(scheme1);
                database.Model.Schemes.Add(scheme2);
                database.Model.Schemes.Add(scheme3);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                int indexOfCompanyA = results.IndexOf(results.First(r => r.OrganisationId == new Guid("659A5E1B-90F8-4E5C-8939-436189424AB6")));
                int indexOfCompanyB = results.IndexOf(results.First(r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4")));
                int indexOfCompanyC = results.IndexOf(results.First(r => r.OrganisationId == new Guid("D7C37279-C3F5-44C0-B6CF-D43A968F3F29")));

                Assert.True(indexOfCompanyA < indexOfCompanyB, "Organisation search results must be returned ordered by organisation name.");
                Assert.True(indexOfCompanyB < indexOfCompanyC, "Organisation search results must be returned ordered by organisation name.");
            }
        }

        [Fact]
        public async Task FetchCompleteOrganisations_OrganisationHasNoRelatedEntities_OrganisationNotReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var organisationGuid = fixture.Create<Guid>();
                var organisation = new Organisation
                {
                    Id = organisationGuid,
                    OrganisationType = Domain.Organisation.OrganisationType.Partnership.Value,
                    OrganisationStatus = Domain.Organisation.OrganisationStatus.Complete.Value
                };

                database.Model.Organisations.Add(organisation);
                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                var results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.DoesNotContain(results, r => r.OrganisationId == organisationGuid);
            }
        }

        [Fact]
        public async Task FetchCompleteOrganisations_OrganisationHasRejectedSchemeWithNoAatfsOrAesOrBalancingScheme_OrganisationNotReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var organisationGuid = fixture.Create<Guid>();
                var organisation = new Organisation
                {
                    Id = organisationGuid,
                    OrganisationType = Domain.Organisation.OrganisationType.Partnership.Value,
                    OrganisationStatus = Domain.Organisation.OrganisationStatus.Complete.Value
                };
                database.Model.Organisations.Add(organisation);

                var scheme = new Scheme
                {
                    Id = fixture.Create<Guid>(),
                    Organisation = organisation,
                    SchemeStatus = (int)Core.Shared.SchemeStatus.Rejected
                };
                database.Model.Schemes.Add(scheme);

                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                var results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.DoesNotContain(results, r => r.OrganisationId == organisationGuid);
            }
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationReturnedWithAddress()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Organisation organisation = new Organisation();
                organisation.Id = new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4");
                organisation.TradingName = "Trading Name";
                organisation.OrganisationType = EA.Weee.Domain.Organisation.OrganisationType.Partnership.Value;
                organisation.OrganisationStatus = EA.Weee.Domain.Organisation.OrganisationStatus.Complete.Value;

                organisation.Address = helper.CreateOrganisationAddress();

                database.Model.Organisations.Add(organisation);

                var scheme = new Scheme
                {
                    Id = new Guid("CFD9B56F-6C3C-4E49-825C-A125ACFFEC3B"),
                    Organisation = organisation,
                    SchemeStatus = (int)Core.Shared.SchemeStatus.Approved
                };
                database.Model.Schemes.Add(scheme);

                database.Model.SaveChanges();

                var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(database.WeeeContext, new AddressMap());

                // Act
                IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

                // Assert
                Assert.Contains(results,
                    r => r.OrganisationId == new Guid("6BD77BBD-0BD8-4BAB-AA9F-A3E657D1CBB4") && r.Name == "Trading Name"
                    && r.Address.Address1 == organisation.Address.Address1
                    && r.Address.Address2 == organisation.Address.Address2
                    && r.Address.TownOrCity == organisation.Address.TownOrCity
                    && r.Address.CountyOrRegion == organisation.Address.CountyOrRegion
                    && r.Address.Postcode == organisation.Address.Postcode);
            }
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithPcs_NoAatfs_OrganisationReturned()
        {
            var dbContextHelper = new DbContextHelper();

            var organisationId = Guid.NewGuid();

            var organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            var organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            var aatfs = new List<Aatf>();

            var schemes = new List<Domain.Scheme.Scheme>()
            {
                new Domain.Scheme.Scheme(organisation)
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            var results = await dataAccess.FetchCompleteOrganisations();

            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationThatIsBalancingScheme_OrganisationReturned()
        {
            var dbContextHelper = new DbContextHelper();

            var organisationId = Guid.NewGuid();

            var organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(A.Dummy<ProducerBalancingScheme>());

            var organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>()));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Domain.Scheme.Scheme>()));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            var results = await dataAccess.FetchCompleteOrganisations();

            results.Count.Should().Be(1);
            results.Should().Contain(o => o.OrganisationId == organisationId);
            results.ElementAt(0).IsBalancingScheme.Should().BeTrue();
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationThatIsNotBalancingScheme_OrganisationShouldBeReturnedWithIsBalancingSchemeAsFalse()
        {
            var dbContextHelper = new DbContextHelper();

            var organisationId = Guid.NewGuid();

            var organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);

            var organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            var schemes = new List<Domain.Scheme.Scheme>()
            {
                new Domain.Scheme.Scheme(organisation)
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>()));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            var results = await dataAccess.FetchCompleteOrganisations();

            results.Count.Should().Be(1);
            results.Should().Contain(o => o.OrganisationId == organisationId);
            results.ElementAt(0).IsBalancingScheme.Should().BeFalse();
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithRejectedPcs_NoAatfs_OrganisationNotReturned()
        {
            var dbContextHelper = new DbContextHelper();

            var organisationId = Guid.NewGuid();

            var organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            var organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            var aatfs = new List<Aatf>();

            var scheme = new Domain.Scheme.Scheme(organisation);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Rejected);
            var schemes = new List<Domain.Scheme.Scheme>()
            {
                scheme
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            var results = await dataAccess.FetchCompleteOrganisations();

            Assert.Empty(results);
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithRejectedPcs_OneAatf_OrganisationReturned()
        {
            var dbContextHelper = new DbContextHelper();

            var organisationId = Guid.NewGuid();

            var organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            var organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            var aatfs = new List<Aatf>()
            {
                new Aatf("one", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), FacilityType.Aatf, 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>())
            };

            var scheme = new Domain.Scheme.Scheme(organisation);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Rejected);
            var schemes = new List<Domain.Scheme.Scheme>()
            {
                scheme
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            var results = await dataAccess.FetchCompleteOrganisations();

            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithPcs_CountReturned()
        {
            DbContextHelper dbContextHelper = new DbContextHelper();

            Guid organisationId = Guid.NewGuid();

            Domain.Organisation.Organisation organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            List<Domain.Organisation.Organisation> organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            List<Aatf> aatfs = new List<Aatf>()
            {
                new Aatf("one", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), A.Dummy<FacilityType>(), 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>()),
                new Aatf("two", A.Dummy<UKCompetentAuthority>(), "5678", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), A.Dummy<FacilityType>(), 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>())
            };

            List<Domain.Scheme.Scheme> schemes = new List<Domain.Scheme.Scheme>()
            {
                new Domain.Scheme.Scheme(organisation)
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

            Assert.Contains(results,
                r => r.PcsCount == 1);
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithAatf_CountReturned()
        {
            DbContextHelper dbContextHelper = new DbContextHelper();

            Guid organisationId = Guid.NewGuid();

            Domain.Organisation.Organisation organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            List<Domain.Organisation.Organisation> organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            List<Aatf> aatfs = new List<Aatf>()
            {
                new Aatf("one", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), FacilityType.Aatf, 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>()),
                new Aatf("two", A.Dummy<UKCompetentAuthority>(), "5678", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), FacilityType.Aatf, 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>())
            };

            List<Domain.Scheme.Scheme> schemes = new List<Domain.Scheme.Scheme>()
            {
                new Domain.Scheme.Scheme(organisation)
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

            Assert.Contains(results,
                r => r.AatfCount == 2);
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithAe_CountReturned()
        {
            DbContextHelper dbContextHelper = new DbContextHelper();

            Guid organisationId = Guid.NewGuid();

            Domain.Organisation.Organisation organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            List<Domain.Organisation.Organisation> organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            List<Aatf> aatfs = new List<Aatf>()
            {
                new Aatf("one", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), FacilityType.Ae, 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>()),
                new Aatf("two", A.Dummy<UKCompetentAuthority>(), "5678", AatfStatus.Approved, organisation, A.Dummy<AatfAddress>(), AatfSize.Large, DateTime.Now, A.Dummy<AatfContact>(), FacilityType.Ae, 2019, A.Dummy<LocalArea>(), A.Dummy<PanArea>())
            };

            List<Domain.Scheme.Scheme> schemes = new List<Domain.Scheme.Scheme>()
            {
                new Domain.Scheme.Scheme(organisation)
            };

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

            Assert.Contains(results,
                r => r.AeCount == 2);
        }

        [Fact]
        public async Task FetchOrganisations_OrganisationWithScheme_OneRejectedScheme_CountReturned()
        {
            DbContextHelper dbContextHelper = new DbContextHelper();

            Guid organisationId = Guid.NewGuid();

            Domain.Organisation.Organisation organisation = A.Dummy<Domain.Organisation.Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => organisation.OrganisationStatus).Returns(Domain.Organisation.OrganisationStatus.Complete);

            List<Domain.Organisation.Organisation> organisations = new List<Domain.Organisation.Organisation>()
            {
                organisation
            };

            List<Domain.Scheme.Scheme> schemes = new List<Domain.Scheme.Scheme>()
            {
                new Domain.Scheme.Scheme(organisation),
                new Domain.Scheme.Scheme(organisation)
            };

            schemes[0].SetStatus(Domain.Scheme.SchemeStatus.Rejected);

            A.CallTo(() => context.Organisations).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisations));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>()));
            A.CallTo(() => context.Schemes).Returns(dbContextHelper.GetAsyncEnabledDbSet(schemes));

            var dataAccess = new FetchOrganisationSearchResultsForCacheDataAccess(context, new AddressMap());

            IList<OrganisationSearchResult> results = await dataAccess.FetchCompleteOrganisations();

            Assert.Contains(results,
                r => r.PcsCount == 1);
        }
    }
}
