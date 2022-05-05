namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using ViewModels.ManageEvidenceNotes;

    public class TransferEvidenceTonnageViewModelMap : TransferEvidenceMapBase<TransferEvidenceTonnageViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>
    {
        private const string EmptyTonnage = "-";
        private readonly ICategoryValueTotalCalculator categoryValueTotalCalculator;

        public TransferEvidenceTonnageViewModelMap(IMapper mapper, IWeeeCache cache, ICategoryValueTotalCalculator categoryValueTotalCalculator) : base(mapper, cache)
        {
            this.categoryValueTotalCalculator = categoryValueTotalCalculator;
        }

        public TransferEvidenceTonnageViewModel Map(TransferEvidenceNotesViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();
            Condition.Requires(source.Request).IsNotNull();
            
            var model = MapBaseProperties(source);

            model.EvidenceNotesDataList =
                model.EvidenceNotesDataList.OrderBy(a => a.SubmittedBy).ThenBy(ab => ab.Id).ToList();

            for (var i = 0; i < model.EvidenceNotesDataList.Count; i++)
            {
                FilterCategories(source, model, i);

                DisplayAatf(i, model);

                SetupTonnages(source, model, i);
            }

            foreach (var category in model.CategoryValues)
            {
                var categories = model.EvidenceNotesDataList.SelectMany(e =>
                    e.CategoryValues.Where(ec => ec.CategoryId.Equals(category.CategoryId)));

                category.TotalReceived = categoryValueTotalCalculator.Total(categories.Select(e1 => e1.Received).ToList());

                category.TotalReused = categoryValueTotalCalculator.Total(categories.Select(e1 => e1.Reused).ToList());
            }
            return model;
        }

        private static void DisplayAatf(int i, TransferEvidenceTonnageViewModel model)
        {
            if (i > 0 && model.EvidenceNotesDataList.Count > 1 && model.EvidenceNotesDataList.ElementAt(i).SubmittedBy
                    .Equals(model.EvidenceNotesDataList.ElementAt(i - 1).SubmittedBy))
            {
                model.EvidenceNotesDataList.ElementAt(i).DisplayAatfName = false;
            }
            else
            {
                model.EvidenceNotesDataList.ElementAt(i).DisplayAatfName = true;
            }
        }

        private static void FilterCategories(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model, int i)
        {
            model.EvidenceNotesDataList.ElementAt(i).CategoryValues =
                model.EvidenceNotesDataList.ElementAt(i).CategoryValues.Where(c =>
                    source.Request.CategoryIds.Contains(c.CategoryId) && !c.Received.Equals(EmptyTonnage)).ToList();
        }

        private static void SetupTonnages(TransferEvidenceNotesViewModelMapTransfer source,
            TransferEvidenceTonnageViewModel model, int i)
        {
            foreach (var evidenceCategoryValue in model.EvidenceNotesDataList.ElementAt(i).CategoryValues)
            {
                var tonnage = new EvidenceCategoryValue((WeeeCategory)evidenceCategoryValue.CategoryId)
                {
                    Id = model.EvidenceNotesDataList.ElementAt(i).Id,
                };

                if (source.TransferAllTonnage)
                {
                    tonnage.Received = evidenceCategoryValue.Received != EmptyTonnage
                        ? evidenceCategoryValue.Received
                        : null;
                    tonnage.Reused = evidenceCategoryValue.Reused != EmptyTonnage
                        ? evidenceCategoryValue.Reused
                        : null;
                }

                model.TransferCategoryValues.Add(tonnage);
            }
        }
    }
}