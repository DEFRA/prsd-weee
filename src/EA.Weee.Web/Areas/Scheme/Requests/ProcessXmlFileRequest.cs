namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Weee.Requests.Scheme.MemberRegistration;

    public class ProcessXmlFileRequest : IMap<PCSFileUploadViewModel, ProcessXmlFile>
    {
        private readonly IFileConverterService fileConverter;

        public ProcessXmlFileRequest(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public ProcessXmlFile Map(PCSFileUploadViewModel source)
        {
            return new ProcessXmlFile(source.PcsId, fileConverter.Convert(source.File), source.File.FileName);
        }
    }
}