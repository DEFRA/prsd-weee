namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;

    public class RadioButtonGenericStringCollectionViewModel<T> : RadioButtonStringCollectionViewModel
    {
        private readonly RadioButtonStringCollectionViewModel model;

        public override IList<string> PossibleValues
        {
            get { return model.PossibleValues; }
            set { model.PossibleValues = value; }
        }

        public sealed override string SelectedValue { get; set; }

        public RadioButtonGenericStringCollectionViewModel()
        {
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException(string.Format("Type '{0}' is not an enum", typeof(T).Name));
            }

            model = CreateFromEnum<T>();
        }
    }
}