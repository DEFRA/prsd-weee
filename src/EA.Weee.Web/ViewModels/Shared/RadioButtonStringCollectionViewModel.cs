namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public class RadioButtonStringCollectionViewModel : RadioButtonStringCollectionBaseViewModel
    {
        [Required(ErrorMessage = "Please answer this question")]
        public override string SelectedValue { get; set; }

        public override IList<string> PossibleValues { get; set; }

        public RadioButtonStringCollectionViewModel()
        {
        }

        public RadioButtonStringCollectionViewModel(IEnumerable<string> stringsToUse)
        {
            this.PossibleValues = stringsToUse.ToList();
        }

        /// <summary>
        /// Creates a radio button string collection based on values in an enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static RadioButtonStringCollectionViewModel CreateFromEnum<T>(string selectedValue = null)
        {
            if (!(typeof(Enum).IsAssignableFrom(typeof(T))))
            {
                throw new InvalidOperationException();
            }

            // Get the enum fields if present.
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            var fieldNames = new List<string>();

            foreach (var field in fields)
            {
                bool selectThisValue = selectedValue != null && selectedValue.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase);

                // Get the display attributes for the enum.
                var displayAttribute = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute)).SingleOrDefault();

                // Set field name to either the enum name or the display name.
                var name = (displayAttribute == null) ? field.Name : displayAttribute.Name;

                if (selectThisValue)
                {
                    selectedValue = name;
                }

                fieldNames.Add(name);
            }

            return new RadioButtonStringCollectionViewModel()
            {
                PossibleValues = fieldNames,
                SelectedValue = selectedValue
            };
        }

        public static RadioButtonStringCollectionViewModel CreateFromEnum<T>(T selectedValue)
        {
            if (!(selectedValue is Enum))
            {
                throw new InvalidOperationException();
            }

            var enumValue = selectedValue as Enum;

            return CreateFromEnum<T>(Enum.GetName(typeof(T), enumValue));
        }
    }
}