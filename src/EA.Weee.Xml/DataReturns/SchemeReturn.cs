namespace EA.Weee.Xml.DataReturns
{
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed")]

    public partial class SchemeReturn
    {
        [XmlElement("XSDVersion")]
        public string xSDVersionString { get; set; }

        /// <remarks/>
        [XmlIgnore]
        public decimal XSDVersion
        {
            get
            {
                if (string.IsNullOrEmpty(xSDVersionString))
                {
                    return 3.23m;
                }
                return decimal.Parse(xSDVersionString);
            }
        }
    }
}
