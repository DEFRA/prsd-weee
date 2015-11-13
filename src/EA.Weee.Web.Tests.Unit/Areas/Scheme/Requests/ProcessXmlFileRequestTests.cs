namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Requests
{
    using System;
    using System.Web;
    using FakeItEasy;
    using Services;
    using Web.Areas.Scheme.Requests;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class ProcessXmlFileRequestTests
    {
        private readonly IFileConverterService fileConverter;

        public ProcessXmlFileRequestTests()
        {
            fileConverter = A.Fake<IFileConverterService>();
        }

        [Fact]
        public void Map_ConvertsFileContentToString()
        {
            var file = A.Fake<HttpPostedFileBase>();

            Request().Map(new AddOrAmendMembersViewModel { File = file });

            A.CallTo(() => fileConverter.Convert(file))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private ProcessXmlFileRequest Request()
        {
            return new ProcessXmlFileRequest(fileConverter);
        }
    }
}
