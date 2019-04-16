namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Helpers;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;

    public class ReturnToObligatedViewModelMap : IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>> obligatedMap;
        private readonly ICategoryValueTotalCalculator calculator;
        private readonly IPasteProcessor pasteProcessor;

        public ReturnToObligatedViewModelMap(IWeeeCache cache,
            IMap<ObligatedDataToObligatedValueMapTransfer,
            IList<ObligatedCategoryValue>> obligatedMap,
            ICategoryValueTotalCalculator calculator,
            IPasteProcessor pasteProcessor)
        {
            this.cache = cache;
            this.obligatedMap = obligatedMap;
            this.calculator = calculator;
            this.pasteProcessor = pasteProcessor;
        }

        public ObligatedViewModel Map(ReturnToObligatedViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ObligatedViewModel(new ObligatedCategoryValues(), calculator)
            {
                SchemeName = (source.SchemeId != new Guid()) ? Task.Run(() => cache.FetchSchemePublicInfoBySchemeId(source.SchemeId)).Result.Name : null,
                AatfName = Task.Run(() => cache.FetchAatfData(source.OrganisationId, source.AatfId)).Result.Name,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                SchemeId = source.SchemeId
            };

            var transfer = new ObligatedDataToObligatedValueMapTransfer() { ObligatedCategoryValues = model.CategoryValues };
            var existingData = new List<ObligatedCategoryValue>();

            if (source.WeeeSentOnId != null)
            {
                model.WeeeSentOnId = source.WeeeSentOnId;
            }

            if (source.SchemeId != new Guid())
            {
                transfer.WeeeDataValues = source.ReturnData.ObligatedWeeeReceivedData.Where(w => w.Aatf.Id == source.AatfId && w.Scheme.Id == source.SchemeId).ToList();
                existingData = obligatedMap.Map(transfer).ToList();
            }
            else if (source.OperatorName != null)
            {
                transfer.WeeeDataValues = source.ReturnData.ObligatedWeeeSentOnData.Where(w => w.WeeeSentOnId == source.WeeeSentOnId).ToList();
                existingData = obligatedMap.Map(transfer).ToList();
            }
            else
            {
                transfer.WeeeDataValues = source.ReturnData.ObligatedWeeeReusedData.Where(w => w.Aatf.Id == source.AatfId).ToList();
                existingData = obligatedMap.Map(transfer).ToList();
            }

            if (source.OperatorName != null)
            {
                model.OperatorName = source.OperatorName;
            }

            if (source.PastedData != null)
            {
                var obligatedPastedValues = new ObligatedPastedValues();

                obligatedPastedValues.B2B = pasteProcessor.BuildModel(source.PastedData.B2B);
                obligatedPastedValues.B2C = pasteProcessor.BuildModel(source.PastedData.B2C);

                model.CategoryValues = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues, existingData);
            }
            else
            {
                model.CategoryValues = existingData;
            }

            return model;
        }
    }
}