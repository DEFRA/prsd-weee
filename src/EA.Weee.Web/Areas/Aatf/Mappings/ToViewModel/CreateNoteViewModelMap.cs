namespace EA.Weee.Web.Areas.AatfEvidence.Mappings.ToViewModel
{
    using System;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using Prsd.Core;
    using Prsd.Core.Domain;
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