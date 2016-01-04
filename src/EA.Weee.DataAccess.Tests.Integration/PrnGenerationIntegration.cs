namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Core.Helpers.PrnGeneration;
    using Domain;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core.Model;
    using Xml.Converter;
    using Xml.Deserialization;
    using Xml.MemberRegistration;
    using Xunit;
    using ChargeBandAmount = Domain.Lookup.ChargeBandAmount;
    using MemberUpload = Domain.Scheme.MemberUpload;
    using MemberUploadError = Domain.Scheme.MemberUploadError;
    using Organisation = Domain.Organisation.Organisation;
    using ProducerSubmission = Domain.Producer.ProducerSubmission;
    using Scheme = Domain.Scheme.Scheme;

    public class PrnGenerationIntegration
    {
        private readonly IUserContext userContext;
        private readonly IEventDispatcher eventDispatcher;

        public PrnGenerationIntegration()
        {
            userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            eventDispatcher = A.Fake<IEventDispatcher>();
        }

        [Fact]
        public async Task HappyPath_ManyUniquePrnsGeneratedAndSeedUpdatedToExpectedValue()
        {
            // arrange
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid-many-insertions.xml");
            var validXmlString = File.ReadAllText(new Uri(validXmlLocation).LocalPath);
            var validXmlBytes = File.ReadAllBytes(new Uri(validXmlLocation).LocalPath);

            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var org = modelHelper.CreateOrganisation();
                var scheme = modelHelper.CreateScheme(org);
                var memberUpload = modelHelper.CreateMemberUpload(scheme);

                var message = new ProcessXmlFile(org.Id, validXmlBytes, "File name");

                var initialSeed = database.WeeeContext.SystemData.Select(sd => sd.LatestPrnSeed).First();
                var expectedSeed = ExpectedSeedAfterThisXml(validXmlString, initialSeed);

                var whiteSpaceCollapser = A.Fake<IWhiteSpaceCollapser>();

                var xmlConverter = new XmlConverter(whiteSpaceCollapser, new Deserializer());
                var schemeType = xmlConverter.Deserialize(xmlConverter.Convert(message.Data));

                var producerCharges = new Dictionary<string, ProducerCharge>();
                var anyAmount = 30;
                var anyChargeBandAmount = A.Dummy<ChargeBandAmount>();

                foreach (var producerData in schemeType.producerList)
                {
                    var producerName = producerData.GetProducerName();
                    if (!producerCharges.ContainsKey(producerName))
                    {
                        producerCharges.Add(producerName,
                            new ProducerCharge { Amount = anyAmount, ChargeBandAmount = anyChargeBandAmount });
                    }
                }

                database.Model.SaveChanges();

                var contextMemberUpload = database.WeeeContext.MemberUploads
                    .Single(mu => mu.Id == memberUpload.Id);

                // act
                var producers = await new GenerateFromXml(
                    xmlConverter,
                    new GenerateFromXmlDataAccess(database.WeeeContext)).GenerateProducers(message, contextMemberUpload, producerCharges);

                // assert
                long newSeed = database.WeeeContext.SystemData.Select(sd => sd.LatestPrnSeed).First();
                Assert.Equal(expectedSeed, newSeed);

                var prns = producers.Select(p => p.RegisteredProducer.ProducerRegistrationNumber);
                Assert.Equal(prns.Distinct(), prns); // all prns should be unique
            }
        }

        [Fact(Skip = "This is a time-consuming test and shouldn't be run automatically")]
        public void StressTest()
        {
            var helper = new PrnHelper(new QuadraticResidueHelper());
            var generatedPrns = new HashSet<string>();

            var context = new WeeeContext(userContext, eventDispatcher);

            uint seed = (uint)GetCurrentSeed(context);
            var components = new PrnAsComponents(seed);

            // be careful how high you go with this limit or generatedPrns will fill up
            // and your computer will get stuck in page-fault limbo
            const int Limit = int.MaxValue / 100;

            for (int ii = 0; ii < Limit; ii++)
            {
                if (ii % (Limit / 10) == 0)
                {
                    Debug.WriteLine("Done another ten per cent...");
                }

                var resultingPrn = helper.CreateUniqueRandomVersionOfPrn(components);

                Assert.False(generatedPrns.Contains(resultingPrn),
                    string.Format(
                    "{0} was generated twice but is supposed to be unique for a very large range of seed values",
                    resultingPrn));

                generatedPrns.Add(resultingPrn);

                seed = components.ToSeedValue() + 1;
                components = new PrnAsComponents(seed);
            }
        }

        private long GetCurrentSeed(WeeeContext context)
        {
            return context.SystemData.Select(sd => sd.LatestPrnSeed).First();
        }

        private long ExpectedSeedAfterThisXml(string xml, long initialSeed)
        {
            schemeType parsedXml = ParseXml(xml);
            uint expectedSeed = (uint)initialSeed;
            for (int ii = 0; ii < parsedXml.producerList.Count(p => p.status == statusType.I); ii++)
            {
                expectedSeed = new PrnAsComponents(expectedSeed + 1).ToSeedValue();
            }
            return expectedSeed;
        }

        private schemeType ParseXml(string xml)
        {
            var doc = XDocument.Parse(xml, LoadOptions.SetLineInfo);
            var deserialzedXml = new XmlSerializer(typeof(schemeType)).Deserialize(doc.CreateReader());
            return (schemeType)deserialzedXml;
        }
    }
}
