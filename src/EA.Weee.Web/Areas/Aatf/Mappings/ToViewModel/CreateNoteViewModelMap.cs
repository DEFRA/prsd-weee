namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class CreateNoteViewModelMap : IMap<CreateNoteMapTransfer, CreateNoteViewModel>
    {
        public CreateNoteViewModel Map(CreateNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new CreateNoteViewModel
            {
                SchemeList = source.Schemes,
                ProtocolList = Enumeration.GetAll<Protocol>(),
                WasteTypeList = Enumeration.GetAll<WasteType>()
            };

            return model;
        }
    }
}