namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString RadioButtonsFor<TValue, T>(
            Expression<Func<TModel, TValue>> expression,
            IEnumerable<T> possibleValues,
            Func<T, object> keySelector,
            Func<T, object> valueSelector,
            RadioButtonLegend legend,
            RadioButtonLayout layout)
        {
            string labelsHtml = string.Empty;
            foreach (T possibleValue in possibleValues)
            {
                MvcHtmlString name = HtmlHelper.NameFor(expression);

                string key = keySelector(possibleValue).ToString();
                string value = valueSelector(possibleValue).ToString();
                
                string id = string.Format("{0}-{1}", name, key);

                string inputHtml = string.Format("<input id=\"{0}\" type=\"radio\" name=\"{1}\" value=\"{2}\">", id, name, key);

                string labelHtml = string.Format("<label class=\"block-label\" for=\"{0}\">{1}{2}</label>", id, inputHtml, value);

                labelsHtml += labelHtml;
            }

            MvcHtmlString legendText = HtmlHelper.DisplayNameFor(expression);

            if (MvcHtmlString.IsNullOrEmpty(legendText))
            {
                string errorMessage = "A legend should always be provided for a radio button selection. " +
                    "Consider adding a display name data annotation to the selected item property.";
                throw new Exception(errorMessage);
            }

            return WrapRadioButtonsInFieldSet(labelsHtml, legendText, legend, layout);
        }

        public MvcHtmlString RadioButtonsFor<TValue>(
            Expression<Func<TModel, TValue>> expression,
            IList<string> possibleValues,
            string legendText,
            RadioButtonLegend legend,
            RadioButtonLayout layout)
        {
            if (string.IsNullOrEmpty(legendText))
            {
                throw new InvalidOperationException("A legend should always be provided for a radio button selection");
            }

            var radioButtonHtml = string.Empty;
            for (var i = 0; i < possibleValues.Count; i++)
            {
                var idForThisButton = string.Format("{0}-{1}", HtmlHelper.NameFor(expression), i);

                var div = string.Format("<div>{0}</div>", HtmlHelper.HiddenFor(m => possibleValues[i]));

                var radioButton = HtmlHelper.RadioButtonFor(expression,
                    possibleValues[i], new { id = idForThisButton }).ToString();

                var display = HtmlHelper.DisplayFor(m => possibleValues[i]);

                var label = string.Format("<label for=\"{0}\" class=\"block-label\">{1}</label>", idForThisButton, radioButton + display);

                radioButtonHtml += div + label;
            }

            return WrapRadioButtonsInFieldSet(radioButtonHtml, new MvcHtmlString(legendText), legend, layout);
        }

        private MvcHtmlString WrapRadioButtonsInFieldSet(
            string radioButtonsHtml,
            MvcHtmlString legendText,
            RadioButtonLegend legend,
            RadioButtonLayout layout)
        {
            string legendClassHtml;
            switch (legend)
            {
                case RadioButtonLegend.Displayed:
                    legendClassHtml = string.Empty;
                    break;

                case RadioButtonLegend.VisuallyHidden:
                    legendClassHtml = "class=\"visuallyhidden\"";
                    break;

                default:
                    throw new NotSupportedException();
            }

            string legendHtml = string.Format("<legend {0}>{1}</legend>", legendClassHtml, legendText);

            string fieldSetClassHtml;
            switch (layout)
            {
                case RadioButtonLayout.Stacked:
                    fieldSetClassHtml = string.Empty;
                    break;

                case RadioButtonLayout.Inline:
                    fieldSetClassHtml = "class=\"inline\"";
                    break;

                default:
                    throw new NotSupportedException();
            }

            string fieldSetHtml = string.Format("<fieldset {0}>{1}{2}</fieldset>", fieldSetClassHtml, legendHtml, radioButtonsHtml);

            return new MvcHtmlString(fieldSetHtml);
        }
    }
}