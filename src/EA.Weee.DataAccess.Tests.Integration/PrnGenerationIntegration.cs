namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
    using RequestHandlers;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.Extensions;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class PrnGenerationIntegration
    {
        private readonly WeeeContext context;

        public PrnGenerationIntegration()
        {
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            context = new WeeeContext(userContext);
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

            XmlConverter xmlConverter = new XmlConverter();
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
            IEnumerable<Producer> producers = await new GenerateFromXml(new XmlConverter(), context).GenerateProducers(message, memberUpload, producerCharges);

            // assert
            long newSeed = GetCurrentSeed();
            Assert.Equal(expectedSeed, newSeed);

            var prns = producers.Select(p => p.RegistrationNumber);
            Assert.Equal(prns.Distinct(), prns); // all prns should be unique
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

            MemberUpload memberUpload = new MemberUpload(org.Id, string.Empty);
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
