namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.MemberRegistration;
    using EA.Weee.Requests.MemberRegistration;
    using EA.Weee.Requests.Tests.Unit.Helpers;
    using FakeItEasy;
    using Xunit;

    public class ValidateXmlFileHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        [Fact]
        public async Task Validation_ValidXml_NoErrors()
        {
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            var fakeContext = SetupFakeWeeeContext();

            MemberUpload addedMemberUpload = null;
            A.CallTo(() => fakeContext.MemberUploads.Add(A<MemberUpload>._))
                .Invokes((MemberUpload m) => addedMemberUpload = m);

            var handler = new ValidateXmlFileHandler(fakeContext);

            await handler.HandleAsync(new ValidateXmlFile(validXml));

            Assert.Empty(addedMemberUpload.Errors);
        }

        [Fact]
        public async Task Validation_NonSchemaXml_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-slightly-invalid.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            var fakeContext = SetupFakeWeeeContext();

            MemberUpload addedMemberUpload = null;
            A.CallTo(() => fakeContext.MemberUploads.Add(A<MemberUpload>._))
                .Invokes((MemberUpload m) => addedMemberUpload = m);

            var handler = new ValidateXmlFileHandler(fakeContext);

            await handler.HandleAsync(new ValidateXmlFile(invalidXml));

            Assert.True(addedMemberUpload.Errors.Exists(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public async Task Validation_CorruptXml_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-badly-damaged.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            var fakeContext = SetupFakeWeeeContext();

            MemberUpload addedMemberUpload = null;
            A.CallTo(() => fakeContext.MemberUploads.Add(A<MemberUpload>._))
                .Invokes((MemberUpload m) => addedMemberUpload = m);

            var handler = new ValidateXmlFileHandler(fakeContext);

            await handler.HandleAsync(new ValidateXmlFile(invalidXml));

            Assert.True(addedMemberUpload.Errors.Exists(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public async Task Validation_NonXmlFile_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\not-xml.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            var fakeContext = SetupFakeWeeeContext();

            MemberUpload addedMemberUpload = null;
            A.CallTo(() => fakeContext.MemberUploads.Add(A<MemberUpload>._))
                .Invokes((MemberUpload m) => addedMemberUpload = m);

            var handler = new ValidateXmlFileHandler(fakeContext);

            await handler.HandleAsync(new ValidateXmlFile(invalidXml));

            Assert.True(addedMemberUpload.Errors.Exists(me => me.ErrorLevel == ErrorLevel.Error));
        }

        private WeeeContext SetupFakeWeeeContext()
        {
            var context = A.Fake<WeeeContext>();
            var memberUploads = helper.GetAsyncEnabledDbSet(new MemberUpload[] { });
            A.CallTo(() => context.MemberUploads).Returns(memberUploads);
            return context;
        }
    }
}