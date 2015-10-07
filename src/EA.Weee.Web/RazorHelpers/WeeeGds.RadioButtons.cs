namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public partial class WeeeGds<TModel>
    {
        public MvcHtmlString RadioButtonsFor<TValue>(Expression<Func<TModel, TValue>> expression, IList<string> possibleValues, string legend, bool legendIsHidden = false)
        {
            if (string.IsNullOrEmpty(legend))
            {
                throw new InvalidOperationException("A legend should always be provided for a radio button selection");
            }

            var validationMessage =
                gdsHelper.ValidationMessageFor(expression, "Please answer this question");

            var radioButtonHtml = string.Empty;
            for (var i = 0; i < possibleValues.Count; i++)
            {
                var idForThisButton = HtmlHelper.NameFor(m => possibleValues[i]) + "-" + i;

                var div = string.Format("<div>{0}</div>",
                    HtmlHelper.HiddenFor(m => possibleValues[i]));

                var radioButton = HtmlHelper.RadioButtonFor(expression,
                    possibleValues[i], new { id = idForThisButton }).ToString();

                var display = HtmlHelper.DisplayFor(m => possibleValues[i]);

                var label = string.Format("<label for={0} class=\"block-label\">{1}</label>", idForThisButton, radioButton + display);

                radioButtonHtml += div + label;
            }

            if (legendIsHidden)
            {
                return new MvcHtmlString(validationMessage + string.Format("<fieldset><legend class=\"hidden-for-screen-reader\">{0}</legend>{1}</fieldset>", legend, radioButtonHtml));
            }
            
            return new MvcHtmlString(validationMessage + string.Format("<fieldset><legend>{0}</legend>{1}</fieldset>", legend, radioButtonHtml));
        }
    }
}