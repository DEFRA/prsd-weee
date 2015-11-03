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
        public string annualTurnoverString { get; set; }

        /// <remarks/>
        [XmlIgnore]
        public float annualTurnover
        {
            get
            {
                if (string.IsNullOrEmpty(annualTurnoverString))
                {
                    return 0F;
                }
                return float.Parse(annualTurnoverString);
            }
            set { annualTurnoverString = value.ToString(CultureInfo.InvariantCulture); }
        }
    }
}
