namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.PCS;
    using Domain.Producer;
    using FakeItEasy;
    using Helpers;
    using PCS.MemberRegistration;
    using Prsd.Core;
    using RequestHandlers.PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration.GenerateProducerObjects;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation;
    using Xunit;

    public class ProcessXMLFileHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly ProcessXMLFileHandler handler;
        private readonly IGenerateFromXml generator;
        private readonly WeeeContext context;
        private readonly DbSet<Scheme> schemesDbSet;
        private readonly DbSet<Producer> producersDbSet;
        private readonly DbSet<MemberUpload> memberUploadsDbSet;
        private readonly IXmlValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXmlChargeBandCalculator xmlChargeBandCalculator;
        private static readonly Guid organisationId = Guid.NewGuid();
        private static readonly ProcessXMLFile Message = new ProcessXMLFile(organisationId, new byte[1]);

        public ProcessXMLFileHandlerTests()
        {
            memberUploadsDbSet = A.Fake<DbSet<MemberUpload>>();
            producersDbSet = A.Fake<DbSet<Producer>>();
            xmlConverter = A.Fake<IXmlConverter>();
            var schemes = new Scheme[]
            {
                FakeSchemeData()
            };

           schemesDbSet = helper.GetAsyncEnabledDbSet(schemes);
            
            context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Schemes).Returns(schemesDbSet);
            A.CallTo(() => context.Producers).Returns(producersDbSet);
            A.CallTo(() => context.MemberUploads).Returns(memberUploadsDbSet);

            generator = A.Fake<IGenerateFromXml>();
            xmlValidator = A.Fake<IXmlValidator>();
            xmlChargeBandCalculator = A.Fake<IXmlChargeBandCalculator>();
            handler = new ProcessXMLFileHandler(context, xmlValidator, generator, xmlConverter, xmlChargeBandCalculator);
        }

        [Fact]
        public async void ProcessXmlfile_ParsesXMLFile_SavesValidProducers()
        {
            IEnumerable<Producer> generatedProducers = new[] { TestProducer("ForestMoonOfEndor") };

            A.CallTo(() => generator.Generate(Message, A<MemberUpload>.Ignored, A<Hashtable>.Ignored))
                .Returns(Task.FromResult(generatedProducers));

            await handler.HandleAsync(Message);

            A.CallTo(() => producersDbSet.AddRange(generatedProducers)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened();
        }

        [Fact]
        public async void ProcessXmlfile_ParsesXMLFile_CalculateXMLChargeBand()
        {
            await handler.HandleAsync(Message);

            A.CallTo(() => xmlChargeBandCalculator.Calculate(Message)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ProcessXmlfile_InvalidXmlfile_NotGenerateProducerObjects()
        {
            IEnumerable<MemberUploadError> errors = new[] { new MemberUploadError(ErrorLevel.Error, "any description") };
            A.CallTo(() => xmlValidator.Validate(Message)).Returns(errors);
            await handler.HandleAsync(Message);

            A.CallTo(producersDbSet).WithAnyArguments().MustNotHaveHappened();
        }

        [Fact]
        public async void ProcessXmlfile_XmlfileWithSameProducerName_NotGenerateProducerObjects()
        {
            List<MemberUploadError> errors = new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Error, "any description")
            };
            A.CallTo(() => xmlChargeBandCalculator.ErrorsAndWarnings).Returns(errors);

            await handler.HandleAsync(Message);

            A.CallTo(producersDbSet).WithAnyArguments().MustNotHaveHappened();
        }

        [Fact]
        public async void ProcessXmlfile_SavesMemberUpload()
        {
            var memberUpload = FakeMemberUploadData();
            var id = await handler.HandleAsync(Message);
            Assert.NotNull(id);
        }

        public static Producer TestProducer(string tradingName)
        {
            return new Producer(Guid.NewGuid(), FakeMemberUploadData(), null, null, SystemTime.UtcNow, 0, true,
                string.Empty, null, tradingName, EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both, ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds, new List<BrandName>(), new List<SICCode>(), ChargeBandType.E);
        }

        public static MemberUpload FakeMemberUploadData()
        {
            return new MemberUpload(organisationId, "FAKE Member Upload DATA");
        }

        public static Scheme FakeSchemeData()
        {
            return new Scheme(organisationId);
        }
    }
}
