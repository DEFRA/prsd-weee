//namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets
//{
//    using System;
//    using System.Collections.Generic;
//    using DataAccess;
//    using Domain;
//    using Domain.Producer;
//    using FakeItEasy;
//    using Helpers;
//    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets;
//    using Xunit;

//    public class ProducerQuerySetTests
//    {
//        private readonly WeeeContext context;
//        private readonly DbContextHelper helper;

//        public ProducerQuerySetTests()
//        {
//            context = A.Fake<WeeeContext>();
//            helper = new DbContextHelper();
//        }

//        [Fact]
//        public void GetLatestProducerForComplianceYearAndScheme_AllParametersMatch_ReturnsProducer()
//        {
//            var schemeOrganisationId = Guid.NewGuid();
//            var registrationNumber = "ABC12345";
//            var complianceYear = 2016;

//            var producer = FakeProducer.Create(ObligationType.Both, registrationNumber, true, schemeOrganisationId, complianceYear);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    producer
//                }));

//            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(registrationNumber, complianceYear.ToString(), schemeOrganisationId);

//            Assert.Equal(producer, result);
//        }

//        [Theory]
//        [InlineData(2016, 2017)]
//        [InlineData(2017, 2016)]
//        public void GetLatestProducerForComplianceYearAndScheme_ComplianceYearDoesNotMatch_ReturnsNull(int thisComplianceYear, int existingComplianceYear)
//        {
//            var schemeOrganisationId = Guid.NewGuid();
//            var registrationNumber = "ABC12345";

//            var producer = FakeProducer.Create(ObligationType.Both, registrationNumber, true, schemeOrganisationId, existingComplianceYear);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    producer
//                }));

//            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(registrationNumber, thisComplianceYear.ToString(), schemeOrganisationId);
           
//            Assert.Null(result);
//        }

//        [Theory]
//        [InlineData("ABC12345", "ABC12346")]
//        [InlineData("ABC12346", "ABC12345")]
//        public void GetLatestProducerForComplianceYearAndScheme_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
//        {
//            var schemeOrganisationId = Guid.NewGuid();
//            var complianceYear = 2016;

//            var producer = FakeProducer.Create(ObligationType.Both, existingPrn, true, schemeOrganisationId, complianceYear);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    producer
//                }));

//            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(thisPrn, complianceYear.ToString(), schemeOrganisationId);

//            Assert.Null(result);
//        }

//        [Theory]
//        [InlineData("ABC12345", "ABC12346")]
//        [InlineData("ABC12346", "ABC12345")]
//        public void GetLatestProducerFromPreviousComplianceYears_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
//        {
//            var producer = FakeProducer.Create(ObligationType.Both, existingPrn, true, Guid.NewGuid(), 2016);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    producer
//                }));

//            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(thisPrn);

//            Assert.Null(result);
//        }

//        [Fact]
//        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesInConsecutiveYears_ReturnsLatestProducerByComplianceYear()
//        {
//            const string prn = "ABC12345";

//            var oldestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2014);
//            var newestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    oldestProducer,
//                    newestProducer
//                }));

//            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

//            Assert.Equal(newestProducer, result);
//        }

//        [Fact]
//        public void GetLatestProducerFromCurrentComplianceYearForAnotherSchemeSameObligationType__ReturnsAnotherSchemeProducer()
//        {
//            const string prn = "ABC12345";
//            Guid schemeOrgId = Guid.NewGuid();
//            var anotherSchemeProducer = FakeProducer.Create(ObligationType.B2B, prn, true, schemeOrgId, 2016);
//            var currentSchemeProducer = FakeProducer.Create(ObligationType.B2B, prn, true, Guid.NewGuid(), 2016);

//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    anotherSchemeProducer,
//                    currentSchemeProducer
//                }));

//            var result = ProducerQuerySet().GetProducerForOtherSchemeAndObligationType(prn, "2016", schemeOrgId, 1);

//            Assert.Equal(anotherSchemeProducer, result);
//        }
//        [Fact]
//        public void
//            GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_ReturnsLatestProducerByUploadDate()
//        {
//            const string prn = "ABC12345";

//            var oldestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);
//            var newestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    oldestProducer,
//                    newestProducer
//                }));

//            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

//            Assert.Equal(newestProducer, result);
//        }

//        [Fact]
//        public void
//            GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_AndOneIn2014_ReturnsLatestProducerByUploadDateIn2015()
//        {
//            const string prn = "ABC12345";

//            var producer2014 = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2014);
//            var oldestProducer2015 = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);
//            var newestProducer2015 = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
//                {
//                    producer2014,
//                    oldestProducer2015,
//                    newestProducer2015
//                }));

//            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

//            Assert.Equal(newestProducer2015, result);
//        }

//        [Theory]
//        [InlineData("ABC12345", "ABC12346")]
//        [InlineData("ABC12346", "ABC12345")]
//        public void GetMigratedProducer_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
//        {
//            var migratedProducer = FakeMigratedProducer(existingPrn);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>
//                {
//                    migratedProducer
//                }));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>()));

//            var result = ProducerQuerySet().GetMigratedProducer(thisPrn);

//            Assert.Null(result);
//        }

//        [Fact]
//        public void GetMigratedProducer_PrnMatches_ReturnsMigratedProducer()
//        {
//            const string prn = "ABC12345";

//            var migratedProducer = FakeMigratedProducer(prn);

//            A.CallTo(() => context.MigratedProducers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>
//                {
//                    migratedProducer
//                }));
//            A.CallTo(() => context.Producers)
//                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>()));

//            var result = ProducerQuerySet().GetMigratedProducer(prn);

//            Assert.Equal(migratedProducer, result);
//        }

//        private MigratedProducer FakeMigratedProducer(string prn)
//        {
//            var producer = A.Fake<MigratedProducer>();
//            A.CallTo(() => producer.ProducerRegistrationNumber)
//                .Returns(prn);

//            return producer;
//        }

//        private ProducerQuerySet ProducerQuerySet()
//        {
//            return new ProducerQuerySet(context);
//        }
//    }
//}
