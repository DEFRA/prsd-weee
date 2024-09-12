namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class EditEeeDataViewModelMap : IMap<SmallProducerSubmissionData, EditEeeDataViewModel>
    {
        public EditEeeDataViewModel Map(SmallProducerSubmissionData source)
        {
            var model = new EditEeeDataViewModel()
            {
                OrganisationId = source.OrganisationData.Id,
                DirectRegistrantId = source.DirectRegistrantId
            };

            for (var i = model.CategoryValues.Count - 1; i >= 0; i--)
            {
                var category = model.CategoryValues.ElementAt(i);

                //var noteTonnage = source.EvidenceNoteData.EvidenceTonnageData.FirstOrDefault(t =>
                //    t.CategoryId.ToInt().Equals(category.CategoryId.ToInt()));

                //if (noteTonnage == null && !source.IncludeAllCategories)
                //{
                //    model.CategoryValues.RemoveAt(i);
                //}
                //else if (noteTonnage != null)
                //{
                //    category.Received = tonnageUtilities.CheckIfTonnageIsNull(noteTonnage.Received);
                //    category.Reused = tonnageUtilities.CheckIfTonnageIsNull(noteTonnage.Reused);
                //    category.Id = noteTonnage.Id;
                //}
            }

            return model;
        }
    }
}