namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberUploadTesting;
    using Xunit;

    public class ProducerListFactoryTests
    {
        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithUnknownOrganisation_ThrowsArgumentException()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>());

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            // Act
            Func<Task<ProducerList>> action = () => producerListFactory.CreateAsync(new ProducerListSettings());

            // Assert
            await Assert.ThrowsAsync(typeof(ArgumentException), action);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithOrganisationHavingMoreThanOneScheme_ThrowsArgumentException()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>()
            {
                new SchemeInfo(),
                new SchemeInfo()
            });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            // Act
            Func<Task<ProducerList>> action = () => producerListFactory.CreateAsync(new ProducerListSettings());

            // Assert
            await Assert.ThrowsAsync(typeof(ArgumentException), action);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithComplianceYear2015_SetsComplianceYear()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>
            {
                new SchemeInfo { ApprovalNumber = "123", TradingName = "Test" }
            });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                ComplianceYear = 2015
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Assert.Equal(2015, producerList.ComplianceYear);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithSchemaVersion306_SetsSchemaVersion()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>
            {
                new SchemeInfo { ApprovalNumber = "123", TradingName = "Test" }
            });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                SchemaVersion = MemberRegistrationSchemaVersion.Version_3_06
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Assert.Equal(MemberRegistrationSchemaVersion.Version_3_06, producerList.SchemaVersion);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With5NewProducers_Creates5Producers()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>
            {
                new SchemeInfo { ApprovalNumber = "123", TradingName = "Test" }
            });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfNewProducers = 5
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Assert.Equal(5, producerList.Producers.Count);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With3ExistingProducers_Creates3Producers()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>
            {
                new SchemeInfo { ApprovalNumber = "123", TradingName = "Test" }
            });
            A.CallTo(() => dataAccess.GetRegistrationNumbers(A<Guid>._, A<int>._, A<int>._)).Returns(
                new List<string>
                {
                    "1",
                    "2",
                    "3"
                });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfExistingProducers = 3,
                ComplianceYear = 2015
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Assert.Equal(3, producerList.Producers.Count);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With5ExistingProducers_AndDatabaseHas4SubmittedProducers_Creates4Producers()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>
            {
                new SchemeInfo { ApprovalNumber = "123", TradingName = "Test" }
            });
            A.CallTo(() => dataAccess.GetRegistrationNumbers(A<Guid>._, A<int>._, A<int>._)).Returns(
                new List<string>
                {
                    "1",
                    "2",
                    "3",
                    "4"
                });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfExistingProducers = 5,
                ComplianceYear = 2015
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Assert.Equal(4, producerList.Producers.Count);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With1NewProducer_And1ExistingProducer_Creates2Producers()
        {
            // Arrange
            var dataAccess = A.Fake<IProducerListFactoryDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeInfo(A<Guid>._)).Returns(new List<SchemeInfo>
            {
                new SchemeInfo { ApprovalNumber = "123", TradingName = "Test" }
            });
            A.CallTo(() => dataAccess.GetRegistrationNumbers(A<Guid>._, A<int>._, A<int>._)).Returns(
                new List<string>
                {
                    "1"
                });

            ProducerListFactory producerListFactory = new ProducerListFactory(dataAccess);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfNewProducers = 1,
                NumberOfExistingProducers = 1,
                ComplianceYear = 2015
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Assert.Equal(2, producerList.Producers.Count);
        }
    }
}
