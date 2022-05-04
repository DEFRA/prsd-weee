namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Linq;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;

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
                model.EvidenceNotesDataList.ElementAt(i).CategoryValues =
                    model.EvidenceNotesDataList.ElementAt(i).CategoryValues.Where(c =>
                        source.Request.CategoryIds.Contains(c.CategoryId) && !c.Received.Equals("-")).ToList();

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
                    model.TransferCategoryValues.Add(new EvidenceCategoryValue()
                    {

                    });
                }
            }

            return model;
        }
    }
}