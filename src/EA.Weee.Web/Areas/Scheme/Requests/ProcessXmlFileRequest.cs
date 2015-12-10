namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Weee.Requests.Scheme.MemberRegistration;

    public class ProcessXMLFileRequest : IMap<PCSFileUploadViewModel, ProcessXMLFile>
    {
        private readonly IFileConverterService fileConverter;

        public ProcessXMLFileRequest(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public ProcessXMLFile Map(PCSFileUploadViewModel source)
        {
            return new ProcessXMLFile(source.PcsId, fileConverter.Convert(source.File), source.File.FileName);
        }
    }
}