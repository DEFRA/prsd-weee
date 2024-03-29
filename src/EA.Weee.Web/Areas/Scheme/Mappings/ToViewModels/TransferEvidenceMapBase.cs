﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using Filters;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;
    using Web.ViewModels.Shared;

    public abstract class TransferEvidenceMapBase<T> where T : TransferEvidenceViewModelBase, new()
    {
        protected readonly IWeeeCache Cache;
        protected readonly IMapper Mapper;
        protected readonly IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> TransferNoteMapper;

        protected TransferEvidenceMapBase(IMapper mapper, IWeeeCache cache, IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel> transferNoteMapper)
        {
            Mapper = mapper;
            Cache = cache;
            TransferNoteMapper = transferNoteMapper;
        }

        protected T MapBaseProperties(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var recipientId = source.Request?.RecipientId ?? source.TransferEvidenceNoteData.RecipientOrganisationData.Id;

            var model = new T()
            {
                PcsId = source.OrganisationId,
                RecipientName = AsyncHelpers.RunSync(async () => await Cache.FetchSchemePublicInfo(recipientId)).Name,
                RecipientId = recipientId,
                ComplianceYear = source.ComplianceYear
            };

            if (source.TransferEvidenceNoteData != null)
            {
                model.ViewTransferNoteViewModel = TransferNoteMapper.Map(
                    new ViewTransferNoteViewModelMapTransfer(source.OrganisationId, source.TransferEvidenceNoteData, null));
            }

            var categoryValues = source.Request != null ? source.Request.CategoryIds : (source.TransferEvidenceNoteData != null ? source.TransferEvidenceNoteData.CategoryIds : new List<int>());

            foreach (var requestCategoryId in categoryValues.OrderBy(c => c.ToInt()))
            {
                model.CategoryValues.Add(new TotalCategoryValue((Core.DataReturns.WeeeCategory)requestCategoryId));
            }

            return model;
        }
    }
}