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

                var radioButtonDiv = "<div class=\"govuk-radios__item\">{0}</div>";

                string inputHtml = string.Format("<input class=\"govuk-radios__input\" id=\"{0}\" type=\"radio\" name=\"{1}\" value=\"{2}\">", id, name, key);

                string labelHtml = string.Format("<label class=\"govuk-label govuk-radios__label\" for=\"{0}\">{2}</label>", id, inputHtml, value);

                radioButtonDiv = string.Format(radioButtonDiv, inputHtml + labelHtml);

                labelsHtml += radioButtonDiv;
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

            var outerDiv = GetRadioButtonsDiv(layout);

            for (var i = 0; i < possibleValues.Count; i++)
            {
                var idForThisButton = string.Format("{0}-{1}", HtmlHelper.NameFor(expression), i);

                var radioButtonDiv = "<div class=\"govuk-radios__item\">{0}</div>"; 

                var radioButton = HtmlHelper.RadioButtonFor(expression,
                    possibleValues[i], new { id = idForThisButton, @class = "govuk-radios__input" }).ToString();

                var display = HtmlHelper.DisplayFor(m => possibleValues[i]);

                var label = string.Format("<label for=\"{0}\" class=\"govuk-label govuk-radios__label\">{1}</label>", idForThisButton,  display);

                radioButtonDiv = string.Format(radioButtonDiv, HtmlHelper.HiddenFor(m => possibleValues[i]) + radioButton + label);

                radioButtonHtml += radioButtonDiv;
            }

            return WrapRadioButtonsInFieldSet(string.Format(outerDiv, radioButtonHtml), new MvcHtmlString(legendText), legend, layout);
        }

        private string GetRadioButtonsDiv(RadioButtonLayout layout)
        {
            var style = string.Empty;

            switch (layout)
            {
                case RadioButtonLayout.Inline:
                    style = "govuk-radios--inline";
                    break;

                case RadioButtonLayout.Stacked:
                    break;

                default:
                    throw new NotSupportedException();
            }

            var classAttribute = string.Format("class=\"govuk-radios {0}\"", style);

            return string.Format("<div {0}>{1}</div>", classAttribute, "{0}");
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
                    legendClassHtml = "class=\"govuk-visually-hidden\"";
                    break;

                default:
                    throw new NotSupportedException();
            }

            string legendHtml = string.Format("<legend {0}>{1}</legend>", legendClassHtml, legendText);

            //string fieldSetClassHtml;
            //switch (layout)
            //{
            //    case RadioButtonLayout.Stacked:
            //        fieldSetClassHtml = "class=\"govuk-fieldset\"";
            //        break;

            //    case RadioButtonLayout.Inline:
            //        fieldSetClassHtml = "class=\"govuk-fieldset inline\"";
            //        break;

            //    default:
            //        throw new NotSupportedException();
            //}

            string fieldSetHtml = string.Format("<fieldset class=\"govuk-fieldset inline\">{0}{1}</fieldset>", legendHtml, radioButtonsHtml);

            return new MvcHtmlString(fieldSetHtml);
        }
    }
}