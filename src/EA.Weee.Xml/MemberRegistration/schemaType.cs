namespace EA.Weee.Xml.MemberRegistration
{
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed")]

    public partial class schemeType
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
                    return 3.07m;
                }
                return decimal.Parse(xSDVersionString);
            }
        }
    }
}
