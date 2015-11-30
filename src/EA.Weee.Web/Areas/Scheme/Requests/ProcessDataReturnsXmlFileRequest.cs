namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Weee.Requests.DataReturns;

    public class ProcessDataReturnsXmlFileRequest : IMap<PCSFileUploadViewModel, ProcessDataReturnsXMLFile>
    {
        private readonly IFileConverterService fileConverter;

        public ProcessDataReturnsXmlFileRequest(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public ProcessDataReturnsXMLFile Map(PCSFileUploadViewModel source)
        {
            return new ProcessDataReturnsXMLFile(source.PcsId, fileConverter.Convert(source.File), source.File.FileName);
        }
    }
}