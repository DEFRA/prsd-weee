﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using Web.ViewModels.Shared;

    public class TransferEvidenceNotesViewModelMap : TransferEvidenceMapBase<TransferEvidenceNotesViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>
    {
        public TransferEvidenceNotesViewModelMap(IMapper mapper, IWeeeCache cache) : base(mapper, cache)
        {
        }

        public TransferEvidenceNotesViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            Condition.Requires(source.Request).IsNotNull();

            var model = MapBaseProperties(source);

            foreach (var requestCategoryId in source.Request.CategoryIds)
            {
                model.CategoryValues.Add(new CategoryValue((Core.DataReturns.WeeeCategory)requestCategoryId));
            }

            foreach (var evidenceNoteData in source.Notes)
            {
                model.SelectedEvidenceNotePairs.Add(new GenericControlPair<Guid, bool>(evidenceNoteData.Id, false));
            }

            return model;
        }
    }
}