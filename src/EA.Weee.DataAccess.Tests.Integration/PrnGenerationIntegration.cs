﻿namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections;
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
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects;
    using Requests.Scheme.MemberRegistration;
    using Xml;
    using Xml.Schemas;
    using Xunit;

    public class PrnGenerationIntegration
    {
        private readonly WeeeContext context;

        public PrnGenerationIntegration()
        {
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            IEventDispatcher eventDispatcher = A.Fake<IEventDispatcher>();

            context = new WeeeContext(userContext, eventDispatcher);
        }

        [Fact]
        public async Task HappyPath_ManyUniquePrnsGeneratedAndSeedUpdatedToExpectedValue()
        {
            // arrange
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid-many-insertions.xml");
            var validXmlString = File.ReadAllText(new Uri(validXmlLocation).LocalPath);
            var validXmlBytes = File.ReadAllBytes(new Uri(validXmlLocation).LocalPath);

            var orgAndMemberUpload = SetUpContext();
            var org = orgAndMemberUpload.Item1;
            var memberUpload = orgAndMemberUpload.Item2;

            ProcessXMLFile message = new ProcessXMLFile(org.Id, validXmlBytes);

            long initialSeed = GetCurrentSeed();
            long expectedSeed = ExpectedSeedAfterThisXml(validXmlString, initialSeed);

            IWhiteSpaceCollapser whiteSpaceCollapser = A.Fake<IWhiteSpaceCollapser>();

            XmlConverter xmlConverter = new XmlConverter(whiteSpaceCollapser);
            var schemeType = xmlConverter.Deserialize(xmlConverter.Convert(message));

            var producerCharges = new Hashtable();
            var anyCharge = 30;
            var anyChargeBand = ChargeBandType.E;

            foreach (var producerData in schemeType.producerList)
            {
                var producerName = producerData.GetProducerName();
                if (!producerCharges.ContainsKey(producerName))
                {
                    producerCharges.Add(producerName,
                        new ProducerCharge { ChargeAmount = anyCharge, ChargeBandType = anyChargeBand });
                }
            }

            // act
            IEnumerable<Producer> producers = await new GenerateFromXml(xmlConverter, context).GenerateProducers(message, memberUpload, producerCharges);

            // assert
            long newSeed = GetCurrentSeed();
            Assert.Equal(expectedSeed, newSeed);

            var prns = producers.Select(p => p.RegistrationNumber);
            Assert.Equal(prns.Distinct(), prns); // all prns should be unique
        }

        [Fact(Skip = "This is a time-consuming test and shouldn't be run automatically")]
        public void StressTest()
        {
            var helper = new PrnHelper(new QuadraticResidueHelper());
            var generatedPrns = new HashSet<string>();

            uint seed = (uint)GetCurrentSeed();
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

        private long GetCurrentSeed()
        {
            return context.SystemData.Select(sd => sd.LatestPrnSeed).First();
        }

        private Tuple<Organisation, MemberUpload> SetUpContext()
        {
            Organisation org = Organisation.CreateSoleTrader("PrnGeneration HappyPath Test");
            context.Organisations.Add(org);
            context.SaveChanges();

            Scheme scheme = new Scheme(org.Id);
            context.Schemes.Add(scheme);
            context.SaveChanges();

            MemberUpload memberUpload = new MemberUpload(org.Id, scheme.Id, string.Empty);
            context.MemberUploads.Add(memberUpload);
            context.SaveChanges();

            return new Tuple<Organisation, MemberUpload>(org, memberUpload);
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
