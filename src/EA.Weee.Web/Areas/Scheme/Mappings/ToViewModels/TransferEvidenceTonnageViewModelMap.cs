namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;
    using ViewModels.ManageEvidenceNotes;

    public class TransferEvidenceTonnageViewModelMap : TransferEvidenceMapBase<TransferEvidenceTonnageViewModel>, IMap<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>
    {
        public TransferEvidenceTonnageViewModelMap(IMapper mapper, IWeeeCache cache) : base(mapper, cache)
        {
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
                //model.EvidenceNotesDataList.ElementAt(i).CategoryValues =
                //    model.EvidenceNotesDataList.ElementAt(i).CategoryValues.Where(c =>
                //        source.Request.CategoryIds.Contains(c.CategoryId) && !c.Received.Equals("-")).ToList();

                model.EvidenceNotesDataList.ElementAt(i).CategoryValues =
                    model.EvidenceNotesDataList.ElementAt(i).CategoryValues.Where(c =>
                        source.Request.CategoryIds.Contains(c.CategoryId)).ToList();

                if (i > 0 && model.EvidenceNotesDataList.Count > 1 && model.EvidenceNotesDataList.ElementAt(i).SubmittedBy.Equals(model.EvidenceNotesDataList.ElementAt(i - 1).SubmittedBy))
                {
                    model.EvidenceNotesDataList.ElementAt(i).DisplayAatfName = false;
                }
                else
                {
                    model.EvidenceNotesDataList.ElementAt(i).DisplayAatfName = true;
                }

                foreach (var evidenceCategoryValue in model.EvidenceNotesDataList.ElementAt(i).CategoryValues)
                {
                    var tonnage = new EvidenceCategoryValue((WeeeCategory)evidenceCategoryValue.CategoryId)
                    {
                        Id = model.EvidenceNotesDataList.ElementAt(i).Id,
                    };

                    if (source.TransferAllTonnage)
                    {
                        tonnage.Received = evidenceCategoryValue.Received != "-"
                            ? evidenceCategoryValue.Received
                            : null;
                        tonnage.Reused = evidenceCategoryValue.Reused != "-"
                            ? evidenceCategoryValue.Reused
                            : null;
                    }

                    model.TransferCategoryValues.Add(tonnage);
                }
            }

            return model;
        }
    }
}