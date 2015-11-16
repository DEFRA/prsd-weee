namespace EA.Weee.Xml.Schemas
{
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Globalization;
    using System.Xml.Serialization;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed")]
    public partial class producerType
    {
        /// <remarks/>
        [XmlElement("annualTurnover")]
        public string annualTurnoverString { get; set; }

        [XmlIgnore]
        public decimal? annualTurnover
        {
            get
            {
                return string.IsNullOrEmpty(annualTurnoverString)
                    ? (decimal?)null
                    : decimal.Round(decimal.Parse(annualTurnoverString), 12);
            }
            set
            {
                annualTurnoverString = value.HasValue 
                    ? value.Value.ToString(CultureInfo.InvariantCulture) 
                    : null;
            }
        }
    }
}
