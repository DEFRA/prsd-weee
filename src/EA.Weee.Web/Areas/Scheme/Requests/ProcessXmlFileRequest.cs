namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Weee.Requests.Scheme.MemberRegistration;

    public class ProcessXmlFileRequest : IMap<AddOrAmendMembersViewModel, ProcessXMLFile>
    {
        private readonly IFileConverterService fileConverter;

        public ProcessXmlFileRequest(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public ProcessXMLFile Map(AddOrAmendMembersViewModel source)
        {
            return new ProcessXMLFile(source.PcsId, fileConverter.Convert(source.File), source.File.FileName);
        }
    }
}