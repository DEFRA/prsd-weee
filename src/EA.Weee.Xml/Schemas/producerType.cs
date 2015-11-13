namespace EA.Weee.Xml.Schemas
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml.Serialization;
    using CustomTypes;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed")]
    public partial class producerType
    {
        /// <remarks/>
        [XmlElement("annualTurnover", typeof(XmlNullable<decimal>))]
        public XmlNullable<decimal> annualTurnover { get; set; }
    }
}
