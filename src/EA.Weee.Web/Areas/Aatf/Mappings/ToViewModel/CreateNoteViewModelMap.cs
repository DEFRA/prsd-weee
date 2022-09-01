namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class CreateNoteViewModelMap : IMap<CreateNoteMapTransfer, EditEvidenceNoteViewModel>
    {
        public EditEvidenceNoteViewModel Map(CreateNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new EditEvidenceNoteViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfId = source.AatfId,
                SchemeList = source.Schemes,
                ProtocolList = new SelectList(EnumHelper.GetOrderedValuesByKey(typeof(Protocol)), "Key", "Value"),
                WasteTypeList = new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value"),
                ComplianceYear = source.ComplianceYear
            };

            if (source.ExistingModel != null)
            {
                model.CategoryValues = source.ExistingModel.CategoryValues;
                model.StartDate = source.ExistingModel.StartDate;
                model.EndDate = source.ExistingModel.EndDate;
                model.WasteTypeValue = source.ExistingModel.WasteTypeValue;
                model.ProtocolValue = source.ExistingModel.ProtocolValue;
                model.RecipientId = source.ExistingModel.RecipientId;
            }

            return model;
        }
    }
}