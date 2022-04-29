namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Filters;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;

    public class TransferEvidenceNotesViewModelMap : IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        public TransferEvidenceNotesViewModelMap(IMapper mapper, IWeeeCache cache)
        {
            this.mapper = mapper;
            this.cache = cache;
        }

        public TransferEvidenceNotesViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new TransferEvidenceNotesViewModel()
            {
                PcsId = source.OrganisationId,
                RecipientName = AsyncHelpers.RunSync(async () => await cache.FetchSchemeName(source.Request.SchemeId))
            };

            foreach (var requestCategoryId in source.Request.CategoryIds)
            {
                model.CategoryValues.Add(new CategoryValue((Core.DataReturns.WeeeCategory)requestCategoryId));
            }

            foreach (var evidenceNoteData in source.Notes)
            {
                model.EvidenceNotesDataList.Add(mapper.Map<ViewEvidenceNoteViewModel>(new ViewEvidenceNoteMapTransfer(evidenceNoteData, null)));
            }

            model.SelectedEvidenceNotes = new List<bool>(new bool[model.EvidenceNotesDataList.Count]);

            return model;
        }
    }
}