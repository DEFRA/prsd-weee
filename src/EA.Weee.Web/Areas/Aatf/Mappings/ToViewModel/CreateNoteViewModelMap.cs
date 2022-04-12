﻿namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class CreateNoteViewModelMap : IMap<CreateNoteMapTransfer, EvidenceNoteViewModel>
    {
        public EvidenceNoteViewModel Map(CreateNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new EvidenceNoteViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfId = source.AatfId,
                SchemeList = source.Schemes,
                ProtocolList = new SelectList(EnumHelper.GetOrderedValues(typeof(Protocol)), "Key", "Value"),
                WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value")
            };

            if (source.ExistingModel != null)
            {
                model.CategoryValues = source.ExistingModel.CategoryValues;
            }

            return model;
        }
    }
}