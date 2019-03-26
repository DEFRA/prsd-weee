namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Error;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Producer.Classification;
    using Domain.Scheme;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Organisations.GetOrganisationOverview.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Prsd.Core;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xml.Converter;
    using Xml.MemberRegistration;
    using Xunit;

    public class ProcessXMLFileHandlerTests
    {
        private readonly IWeeeAuthorization permissiveAuthorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();
        private readonly IWeeeAuthorization denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly ProcessXMLFileHandler handler;
        private readonly IGenerateFromXml generator;
        private readonly WeeeContext context;
        private readonly DbSet<Scheme> schemesDbSet;
        private readonly DbSet<ProducerSubmission> producersDbSet;
        private readonly DbSet<MemberUpload> memberUploadsDbSet;
        private readonly IXMLValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IProducerSubmissionDataAccess producerSubmissionDataAccess;
        private static readonly Guid organisationId = Guid.NewGuid();
        private static readonly ProcessXmlFile Message = new ProcessXmlFile(organisationId, new byte[1], "File name");
        private readonly ITotalChargeCalculator totalChargeCalculator;
        private readonly ITotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess;

        public ProcessXMLFileHandlerTests()
        {
            memberUploadsDbSet = A.Fake<DbSet<MemberUpload>>();
            producersDbSet = A.Fake<DbSet<ProducerSubmission>>();
            xmlConverter = A.Fake<IXmlConverter>();
            producerSubmissionDataAccess = A.Fake<IProducerSubmissionDataAccess>();
            var schemes = new[]
            {
                FakeSchemeData()
            };

            schemesDbSet = helper.GetAsyncEnabledDbSet(schemes);

            context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Schemes).Returns(schemesDbSet);
            A.CallTo(() => context.ProducerSubmissions).Returns(producersDbSet);
            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            generator = A.Fake<IGenerateFromXml>();
            xmlValidator = A.Fake<IXMLValidator>();
            xmlChargeBandCalculator = A.Fake<IXMLChargeBandCalculator>();
            totalChargeCalculator = A.Fake<ITotalChargeCalculator>();
            totalChargeCalculatorDataAccess = A.Fake<ITotalChargeCalculatorDataAccess>();

            handler = new ProcessXMLFileHandler(context, permissiveAuthorization, xmlValidator, generator, xmlConverter, xmlChargeBandCalculator, producerSubmissionDataAccess, totalChargeCalculator, totalChargeCalculatorDataAccess);
        }

        [Fact]
        public async void NotOrganisationUser_ThrowsSecurityException()
        {
            var authorisationDeniedHandler = new ProcessXMLFileHandler(context, denyingAuthorization, xmlValidator, generator, xmlConverter, xmlChargeBandCalculator, producerSubmissionDataAccess, totalChargeCalculator, totalChargeCalculatorDataAccess);

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await authorisationDeniedHandler.HandleAsync(Message));
        }

        [Fact]
        public async void ProcessXmlfile_ParsesXMLFile_SavesValidProducers()
        {
            IEnumerable<ProducerSubmission> generatedProducers = new[] { TestProducer("ForestMoonOfEndor") };

            A.CallTo(() => generator.GenerateProducers(Message, A<MemberUpload>.Ignored, A<Dictionary<string, ProducerCharge>>.Ignored))
                .Returns(Task.FromResult(generatedProducers));
            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => producerSubmissionDataAccess.AddRange(generatedProducers)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened();
        }
        
        [Fact]
        public async void TotalChargeCalculator_Returns_ProducerCharges()
        {
            var producerCharges = new Dictionary<string, ProducerCharge>();
            var anyAmount = 30;
            var testproducer = "Test Producer";
            var anyChargeBandAmount = A.Dummy<ChargeBandAmount>();
            producerCharges.Add(testproducer, new ProducerCharge { Amount = anyAmount, ChargeBandAmount = anyChargeBandAmount });

            SetupSchemeTypeComplianceYear();

            var hasAnnualCharge = false;
            decimal? totalCharges = 0;

            await handler.HandleAsync(Message);

            A.CallTo(() => totalChargeCalculator.TotalCalculatedCharges(Message, A<Scheme>.Ignored, A<int>.Ignored, hasAnnualCharge, ref totalCharges))
                .Returns(producerCharges);
        }

        [Fact]
        public async void ProcessXmlfile_ParsesXMLFile_CalculateTotalCharges()
        {
            decimal? totalCharges = 0;

            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => totalChargeCalculator.TotalCalculatedCharges(Message, A<Scheme>.Ignored, A<int>.Ignored, A<bool>._, ref totalCharges)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void ProcessXmlfile_GivenSchemeNeedsAnnualCharges_CalculateTotalCharges(bool annualCharge)
        {
            decimal? totalCharges = 0;

            A.CallTo(() => totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(A<Scheme>._, A<int>._)).Returns(annualCharge);

            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => totalChargeCalculator.TotalCalculatedCharges(Message, A<Scheme>.Ignored, A<int>.Ignored, !annualCharge, ref totalCharges)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ProcessXmlfile_SchemaErrors_DoesntTryToCalculateCharges()
        {
            IEnumerable<MemberUploadError> errors = new[]
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Schema, "any description")
            };

            var hasAnnualCharge = false;
            decimal? totalCharges = 0;

            A.CallTo(() => xmlValidator.Validate(Message)).Returns(errors);
            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => totalChargeCalculator.TotalCalculatedCharges(Message, A<Scheme>.Ignored, A<int>.Ignored, hasAnnualCharge, ref totalCharges)).MustNotHaveHappened();
        }

        [Fact]
        public async void ProcessXmlfile_BusinessErrors_TriesToCalculateCharges()
        {
            IEnumerable<MemberUploadError> errors = new[]
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Business, "any description"),
            };

           var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), A.Dummy<string>(), "EA", A.Dummy<Country>(), A.Dummy<string>(), 100);

            var scheme = schemesDbSet.ElementAt(0);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                competentAuthority);

            decimal? totalCharges = 0;

            SetupSchemeTypeComplianceYear();
            A.CallTo(() => xmlValidator.Validate(Message)).Returns(errors);
            
            await handler.HandleAsync(Message);

            A.CallTo(() => totalChargeCalculator.TotalCalculatedCharges(Message, scheme, 2019, true, ref totalCharges)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ProcessXmlfile_NoErrors_TriesToCalculateCharges()
        {
            IEnumerable<MemberUploadError> errors = new List<MemberUploadError>();

            decimal? totalCharges = 0;

            SetupSchemeTypeComplianceYear();

            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), A.Dummy<string>(), "EA", A.Dummy<Country>(), A.Dummy<string>(), 100);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            A.CallTo(() => xmlValidator.Validate(Message)).Returns(errors);
            await handler.HandleAsync(Message);

            A.CallTo(() => totalChargeCalculator.TotalCalculatedCharges(Message, A<Scheme>.Ignored, A<int>.Ignored, A<bool>._, ref totalCharges)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ProcessXmlfile_InvalidXmlfile_NotGenerateProducerObjects()
        {
            IEnumerable<MemberUploadError> errors = new[]
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Schema, "any description")
            };
            SetupSchemeTypeComplianceYear();
            A.CallTo(() => xmlValidator.Validate(Message)).Returns(errors);
            await handler.HandleAsync(Message);

            A.CallTo(generator).Where(g => g.Method.Name == "GenerateProducers").MustNotHaveHappened();
        }

        [Fact]
        public async void ProcessXmlfile_XmlfileWithBusinessError_NotGenerateProducerObjects()
        {
            var errors = new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Business, "any description")
            };
            A.CallTo(() => xmlValidator.Validate(Message)).Returns(errors);

            SetupSchemeTypeComplianceYear();
            await handler.HandleAsync(Message);

            A.CallTo(generator).Where(g => g.Method.Name == "GenerateProducers").MustNotHaveHappened();
        }

        [Fact]
        public async void ProcessXmlfile_HasNoValidationErrors_HasProducerChargeCalculationErrors_ThrowsException()
        {
            var errors = new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Business, "any description")
            };
         
            SetupSchemeTypeComplianceYear();

            A.CallTo(() => xmlChargeBandCalculator.ErrorsAndWarnings).Returns(errors);
            await Assert.ThrowsAsync<ApplicationException>(async () => await handler.HandleAsync(Message));
        }

        [Fact]
        public async void ProcessXmlfile_StoresProcessTime()
        {
            IEnumerable<MemberUploadError> errors = new List<MemberUploadError>();
            A.CallTo(() => xmlValidator.Validate(Message))
                .Returns(errors);

            MemberUpload upload = A.Fake<MemberUpload>();

            A.CallTo(() => generator.GenerateMemberUpload(
                Message,
                errors as List<MemberUploadError>,
                0,
                A.Dummy<Scheme>(), false))
                .WithAnyArguments()
                .Returns(upload);

            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => upload.SetProcessTime(new TimeSpan()))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ProcessXmlfile_SavesMemberUpload()
        {
            SetupSchemeTypeComplianceYear();
            var id = await handler.HandleAsync(Message);
            Assert.NotNull(id);
        }

        [Fact]
        public async void ProcessXmlfile_GivenSchemeDoesNotHaveAnnualCharge_MemberUploadAnnualChargePropertyShouldBeSet()
        {
            A.CallTo(() => totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(A<Scheme>._, A<int>._)).Returns(false);
            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => generator.GenerateMemberUpload(A<ProcessXmlFile>._, A<List<MemberUploadError>>._, A<decimal>._, A<Scheme>._, true))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ProcessXmlfile_GivenSchemeDoesHaveAnnualCharge_MemberUploadAnnualChargePropertyShouldBeSet()
        {
            A.CallTo(() => totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(A<Scheme>._, A<int>._)).Returns(true);
            SetupSchemeTypeComplianceYear();

            await handler.HandleAsync(Message);

            A.CallTo(() => generator.GenerateMemberUpload(A<ProcessXmlFile>._, A<List<MemberUploadError>>._, A<decimal>._, A<Scheme>._, false))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public static ProducerSubmission TestProducer(string tradingName)
        {
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.SchemeName).Returns("Scheme Name");

            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload.Scheme).Returns(scheme);

            var registeredProducer = A.Fake<EA.Weee.Domain.Producer.RegisteredProducer>();
            A.CallTo(() => registeredProducer.ComplianceYear).Returns(2017);
            A.CallTo(() => registeredProducer.Scheme).Returns(scheme);
            A.CallTo(() => registeredProducer.ProducerRegistrationNumber).Returns("WEE/AA1111AA");

            return new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Fake<ProducerBusiness>(),
                null,
                SystemTime.UtcNow,
                0,
                true,
                null,
                tradingName,
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                (decimal)30.0,
                A.Dummy<StatusType>());
        }

        public static Scheme FakeSchemeData()
        {
            var scheme = new Scheme(organisationId);

            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                new UKCompetentAuthority(Guid.NewGuid(), A.Dummy<string>(), "EA", A.Dummy<Country>(), A.Dummy<string>(), 0));

            return scheme;
        }

        private void SetupSchemeTypeComplianceYear()
        {
            var schemeType = new schemeType() { complianceYear = "2019" };
            A.CallTo(() => xmlConverter.Deserialize<schemeType>(A<System.Xml.Linq.XDocument>._)).Returns(schemeType);
        }
    }
}
