namespace EA.Weee.Web.RazorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using Extensions;

    public class TableBuilder<TModel, TDataset> : IDisposable
    {
        private readonly IEnumerable<TDataset> dataset;
        private readonly WeeeGds<TModel> gdsHelper;
        private readonly Dictionary<string, Expression<Func<TDataset, object>>> columns;

        private readonly TagBuilder captionTagBuilder;
        private readonly object htmlAttributes;

        public TableBuilder(WeeeGds<TModel> gdsHelper, 
            IEnumerable<TDataset> dataset, 
            string caption,
            object htmlAttributes = null)
        {
            this.dataset = dataset;
            this.gdsHelper = gdsHelper;
            this.htmlAttributes = htmlAttributes;

            // Initialise caption builder
            var spanTagBuilder = new TagBuilder("span");
            spanTagBuilder.AddCssClass("visually-hidden");
            spanTagBuilder.SetInnerText(caption);

            captionTagBuilder = new TagBuilder("caption") { InnerHtml = spanTagBuilder.ToString() };

            columns = new Dictionary<string, Expression<Func<TDataset, object>>>();
        }

        public void AddColumn(string header, Expression<Func<TDataset, object>> data)
        {
            columns.Add(header, data);
        }

        public void Dispose()
        {
            var headerTagBuilder = BuildTableHeader();
            var bodyTagBuilder = BuildTableBody();

            var tableTagBuilder = new TagBuilder("table")
            {
                InnerHtml = captionTagBuilder.ToString()
                            + headerTagBuilder.ToString()
                            + bodyTagBuilder.ToString()
            };

            tableTagBuilder.AddHtmlAttributes(htmlAttributes);

            gdsHelper.HtmlHelper.ViewContext.Writer.Write(tableTagBuilder.ToString());
        }

        private TagBuilder BuildTableBody()
        {
            var dataRowsHtml = string.Empty;
            foreach (var dataRow in dataset)
            {
                var dataHtml = string.Empty;
                foreach (var columnData in columns)
                {
                    var dataTagBuilder = new TagBuilder("td");
                    dataTagBuilder.SetInnerText(columnData.Value.Compile().Invoke(dataRow).ToString());

                    dataHtml += dataTagBuilder.ToString();
                }

                dataRowsHtml += new TagBuilder("tr") { InnerHtml = dataHtml }.ToString();
            }

            return new TagBuilder("tbody") { InnerHtml = dataRowsHtml };
        }

        private TagBuilder BuildTableHeader()
        {
            var headers = columns.Select(c => c.Key);

            var headersHtml = string.Empty;
            foreach (var header in headers)
            {
                var headingBuilder = new TagBuilder("th");
                headingBuilder.SetInnerText(header);

                headersHtml += headingBuilder.ToString();
            }

            var headerRowBuilder = new TagBuilder("tr") { InnerHtml = headersHtml };
            return new TagBuilder("thead") { InnerHtml = headerRowBuilder.ToString() };
        }
    }
}