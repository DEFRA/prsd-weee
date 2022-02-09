﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Requests
{
    using FakeItEasy;
    using Services;
    using System.Web;
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

            Request().Map(new PCSFileUploadViewModel { File = file });

            A.CallTo(() => fileConverter.Convert(file))
                .MustHaveHappened(1, Times.Exactly);
        }

        private ProcessXmlFileRequest Request()
        {
            return new ProcessXmlFileRequest(fileConverter);
        }
    }
}
