namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class CreateNoteViewModelMap : IMap<CreateNoteMapTransfer, EvidenceNoteViewModel>
    {
        public EvidenceNoteViewModel Map(CreateNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new EvidenceNoteViewModel
            {
                SchemeList = source.Schemes,
                ProtocolList = Enumeration.GetAll<Protocol>(),
                WasteTypeList = Enumeration.GetAll<WasteType>()
            };

            if (source.ExistingModel != null)
            {
                model.CategoryValues = source.ExistingModel.CategoryValues;
            }

            return model;
        }
    }
}