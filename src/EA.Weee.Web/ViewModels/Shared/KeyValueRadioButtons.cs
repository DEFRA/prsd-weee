namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Abstract base class for RadioButton types.
    /// </summary>
    /// <typeparam name="TKey">The type of the text to display by the radio button.</typeparam>
    /// <typeparam name="TValue">The type of the value of the radio button when selected.</typeparam>
    public abstract class KeyValueRadioButtons<TKey, TValue>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public KeyValueRadioButtons()
        {
            PossibleValues = new List<RadioButtonPair<TKey, TValue>>();
        }

        /// <summary>
        /// Construct a RadioButtons class from an IEnumerable of KeyValue pairs of the correct type.
        /// Inheriting classes should call through to this implementation where applicable or provide a custom overload.
        /// </summary>
        /// <param name="keysAndValues">The enumerable of KeyValue pairs to use for PossibleValues.</param>
        public KeyValueRadioButtons(IEnumerable<KeyValuePair<TKey, TValue>> keysAndValues)
        {
            var possibleValues = new List<RadioButtonPair<TKey, TValue>>();

            foreach (var kvp in keysAndValues)
            {
                possibleValues.Add(new RadioButtonPair<TKey, TValue>(kvp));
            }

            PossibleValues = possibleValues;
        }

        /// <summary>
        /// The RadioButtonPairs to use for the list of radio buttons.
        /// </summary>
        public virtual IList<RadioButtonPair<TKey, TValue>> PossibleValues { get; set; }

        /// <summary>
        /// The selected RadioButtonPair when a selection has been made.
        /// </summary>
        public virtual RadioButtonPair<TKey, TValue> Selected
        {
            get
            {
                if (SelectedValue == null)
                {
                    return null;
                }
                else
                {
                    return PossibleValues.FirstOrDefault(pv => pv.Value.Equals(SelectedValue));
                }
            }
            set { SelectedValue = value.Value; }
        }

        /// <summary>
        /// The value passed to the controller from the view, used to populate the Selected RadioButtonPair.
        /// </summary>
        public virtual TValue SelectedValue { get; set; }
    }
}