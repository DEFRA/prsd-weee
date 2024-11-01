namespace EA.Weee.Web.Infrastructure
{
    using EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    
    public class ProducerDataViewModelSchemeDisplayMetaDataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            if (containerType == typeof(ProducersDataViewModel) && propertyName == nameof(ProducersDataViewModel.SelectedSchemeId))
            {
                // Get the model instance
                if (modelAccessor?.Invoke() is ProducersDataViewModel model)
                {
                    metadata.DisplayName = model.GetSchemeDisplayName();
                }
            }

            return metadata;
        }
    }
}