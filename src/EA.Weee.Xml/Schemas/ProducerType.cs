namespace EA.Weee.Xml.Schemas
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml.Serialization;
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed")]
    public partial class producerType
    {
        [XmlElement("annualTurnover")]
        public string annualTurnoverField { get; set; }

        /// <remarks/>
        [XmlIgnore]
        public float annualTurnover
        {
            get
            {
                if (string.IsNullOrEmpty(annualTurnoverField))
                {
                    return 0F;
                }
                return float.Parse(annualTurnoverField);
            }
            set { annualTurnoverField = value.ToString(CultureInfo.InvariantCulture); }
        }
    }
}
