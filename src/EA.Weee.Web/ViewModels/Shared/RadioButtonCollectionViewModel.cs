namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic base class for radio button collections.
    /// Implementing classes must provide a custom editor template.
    /// </summary>
    /// <typeparam name="T">Type of radio button collection.</typeparam>
    public abstract class RadioButtonCollection<T>
    {
        public virtual T SelectedValue { get; set; }

        public virtual IList<T> PossibleValues { get; set; }
    }
}