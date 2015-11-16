namespace EA.Weee.Xml.Schemas
{
    using System.Diagnostics.CodeAnalysis;
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
                if (string.IsNullOrEmpty(annualTurnoverString))
                {
                    return null;
                }

                return decimal.Parse(annualTurnoverString);
            }
            set
            {
                if (value.HasValue)
                {
                    annualTurnoverString = value.Value.ToString(CultureInfo.InvariantCulture);
                }

                annualTurnoverString = null;
            }
        }
    }
}
