namespace EA.Weee.RequestHandlers.Tests.Unit.PCS.MemberUploadTesting
{
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.PCS;
    using EA.Weee.RequestHandlers.PCS.MemberUploadTesting;
    using EA.Weee.Requests.Tests.Unit.Helpers;
    using FakeItEasy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ProducerListFactoryTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        /// <summary>
        /// Sets up a faked WeeeContext with 2 schemes each with 4 submitted producers and 1 unsubmitted producer.
        /// </summary>
        /// <returns></returns>
        private WeeeContext CreateFakeDatabase()
        {
            MemberUpload memberUploadSubmitted = A.Fake<MemberUpload>();
            A.CallTo(() => memberUploadSubmitted.OrganisationId).Returns(new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"));
            A.CallTo(() => memberUploadSubmitted.IsSubmitted).Returns(true);

            MemberUpload memberUploadUnsubmitted = A.Fake<MemberUpload>();
            A.CallTo(() => memberUploadUnsubmitted.OrganisationId).Returns(new Guid("7A6C4AD4-A357-4458-8528-0D0F52859689"));
            A.CallTo(() => memberUploadUnsubmitted.IsSubmitted).Returns(false);

            Domain.Producer.Producer producer1 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer1.RegistrationNumber).Returns("Test Registration Number 1");

            Domain.Producer.Producer producer2 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer2.RegistrationNumber).Returns("Test Registration Number 2");

            Domain.Producer.Producer producer3 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer3.RegistrationNumber).Returns("Test Registration Number 3");

            Domain.Producer.Producer producer4 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer4.RegistrationNumber).Returns("Test Registration Number 4");

            Domain.Producer.Producer producer5 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer5.RegistrationNumber).Returns("Test Registration Number 5");

            Domain.Producer.Producer producer6 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer1.RegistrationNumber).Returns("Test Registration Number 6");

            Domain.Producer.Producer producer7 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer2.RegistrationNumber).Returns("Test Registration Number 7");

            Domain.Producer.Producer producer8 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer3.RegistrationNumber).Returns("Test Registration Number 8");

            Domain.Producer.Producer producer9 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer4.RegistrationNumber).Returns("Test Registration Number 9");

            Domain.Producer.Producer producer10 = A.Fake<Domain.Producer.Producer>();
            A.CallTo(() => producer5.RegistrationNumber).Returns("Test Registration Number 10");
            A.CallTo(() => producer1.MemberUpload).Returns(memberUploadUnsubmitted);

            Organisation organisation1 = A.Fake<Organisation>();
            A.CallTo(() => organisation1.TradingName).Returns("Test Trading Name 1");

            Organisation organisation2 = A.Fake<Organisation>();
            A.CallTo(() => organisation2.TradingName).Returns("Test Trading Name 2");

            Scheme scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.OrganisationId).Returns(new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"));
            A.CallTo(() => scheme1.ApprovalNumber).Returns("Test Approval Number 1");

            Scheme scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.OrganisationId).Returns(new Guid("7A6C4AD4-A357-4458-8528-0D0F52859689"));
            A.CallTo(() => scheme2.ApprovalNumber).Returns("Test Approval Number 2");

            // Wire up scheme to organisations (1-way).
            A.CallTo(() => scheme1.Organisation).Returns(organisation1);
            A.CallTo(() => scheme2.Organisation).Returns(organisation2);

            // Wire up member uploads to organisations (1-way)
            A.CallTo(() => memberUploadSubmitted.Organisation).Returns(organisation1);
            A.CallTo(() => memberUploadUnsubmitted.Organisation).Returns(organisation2);

            // Wire up member uploads to schemes (1-way)
            A.CallTo(() => memberUploadSubmitted.Scheme).Returns(scheme1);
            A.CallTo(() => memberUploadUnsubmitted.Scheme).Returns(scheme2);

            // Wire up producers and schemes (2-way).
            A.CallTo(() => scheme1.Producers).Returns(new List<Domain.Producer.Producer>()
            {
                producer1,
                producer2,
                producer3,
                producer4,
                producer5
            });

            A.CallTo(() => scheme2.Producers).Returns(new List<Domain.Producer.Producer>()
            {
                producer6,
                producer7,
                producer8,
                producer9,
                producer10
            });

            A.CallTo(() => producer1.Scheme).Returns(scheme1);
            A.CallTo(() => producer2.Scheme).Returns(scheme1);
            A.CallTo(() => producer3.Scheme).Returns(scheme1);
            A.CallTo(() => producer4.Scheme).Returns(scheme1);
            A.CallTo(() => producer5.Scheme).Returns(scheme1);
            A.CallTo(() => producer6.Scheme).Returns(scheme2);
            A.CallTo(() => producer7.Scheme).Returns(scheme2);
            A.CallTo(() => producer8.Scheme).Returns(scheme2);
            A.CallTo(() => producer9.Scheme).Returns(scheme2);
            A.CallTo(() => producer10.Scheme).Returns(scheme2);

            // Wire up producers and member uploads (2-way).
            A.CallTo(() => memberUploadSubmitted.Producers).Returns(new List<Domain.Producer.Producer>()
                {
                    producer1,
                    producer2,
                    producer3,
                    producer4,
                    producer6,
                    producer7,
                    producer8,
                    producer9,
                });
            A.CallTo(() => memberUploadUnsubmitted.Producers).Returns(new List<Domain.Producer.Producer>()
                {
                    producer5,
                    producer10,
                });

            A.CallTo(() => producer1.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer2.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer3.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer4.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer5.MemberUpload).Returns(memberUploadUnsubmitted);
            A.CallTo(() => producer6.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer7.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer8.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer9.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer10.MemberUpload).Returns(memberUploadUnsubmitted);
            
            // Wire up everything to the context (1-way).
            WeeeContext weeeContext = A.Fake<WeeeContext>();

            var schemesDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<Scheme>()
                {
                    scheme1,
                    scheme2 
                });
            A.CallTo(() => weeeContext.Schemes).Returns(schemesDbSet);

            var producersDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<Domain.Producer.Producer>()
                {
                    producer1,
                    producer2,
                    producer3,
                    producer4,
                    producer5,
                    producer6,
                    producer7,
                    producer8,
                    producer9,
                    producer10,
                });
            A.CallTo(() => weeeContext.Producers).Returns(producersDbSet);

            var organisationDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation>()
                {
                    organisation1,
                    organisation2
                });
            A.CallTo(() => weeeContext.Organisations).Returns(organisationDbSet);

            var memberUploadDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<MemberUpload>()
                {
                    memberUploadSubmitted,
                    memberUploadUnsubmitted
                });
            A.CallTo(() => weeeContext.MemberUploads).Returns(memberUploadDbSet);

            return weeeContext;
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithUnknownOrganisation_ThrowsException()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();

            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("{33E07014-17E3-4AD6-8F5B-38B3FA568B40}"),
            };

            // Act
            Func<Task<ProducerList>> action = () => producerListFactory.CreateAsync(listSettings);

            // Assert
            await Xunit.Assert.ThrowsAsync(typeof(Exception), action);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithComplianceYear2015_SetsComplianceYear()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();

            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                ComplianceYear = 2015
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Xunit.Assert.Equal(2015, producerList.ComplianceYear);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_WithSchemaVersion306_SetsSchemaVersion()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();

            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                SchemaVersion = SchemaVersion.Version_3_06
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Xunit.Assert.Equal(SchemaVersion.Version_3_06, producerList.SchemaVersion);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With5NewProducers_Creates5Producers()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();

            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfNewProducers = 5
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Xunit.Assert.Equal(5, producerList.Producers.Count);
        }

        [Fact]
        [TestProperty("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With3ExistingProducers_Creates3Producers()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();
            
            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfExistingProducers = 3
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Xunit.Assert.Equal(3, producerList.Producers.Count);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With5ExistingProducers_AndDatabaseHas4SubmittedProducers_Creates4Producers()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();

            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfExistingProducers = 5
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Xunit.Assert.Equal(4, producerList.Producers.Count);
        }

        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public async void CreateProducerList_With1NewProducer_And1ExistingProducer_Creates2Producers()
        {
            // Arrange
            WeeeContext weeeContext = CreateFakeDatabase();

            ProducerListFactory producerListFactory = new ProducerListFactory(weeeContext);

            ProducerListSettings listSettings = new ProducerListSettings()
            {
                OrganisationID = new Guid("20C569E6-C4A0-40C2-9D87-120906D5434B"),
                NumberOfNewProducers = 1,
                NumberOfExistingProducers = 1
            };

            // Act
            ProducerList producerList = await producerListFactory.CreateAsync(listSettings);

            // Assert
            Xunit.Assert.Equal(2, producerList.Producers.Count);
        }
    }
}
