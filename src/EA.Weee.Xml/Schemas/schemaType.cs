namespace EA.Weee.Xml.Schemas
{
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed")]

    public partial class schemeType
    {
        [XmlElement("XSDVersion")]
        public string xSDVersionField { get; set; }

        /// <remarks/>
        [XmlIgnore]
        public decimal XSDVersion
        {
            get
            {
                if (string.IsNullOrEmpty(xSDVersionField))
                {
                    return 3.07m;
                }
                return decimal.Parse(xSDVersionField);
            }
        }
    }
}
