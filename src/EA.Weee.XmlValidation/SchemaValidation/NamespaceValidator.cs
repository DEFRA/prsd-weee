namespace EA.Weee.XmlValidation.SchemaValidation
{
    using Errors;
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Core.Shared;
    using Xml;

    public class NamespaceValidator : INamespaceValidator
    {
        private const string InvalidNamespaceMessageTemplate = "The namespace of the XML file you have uploaded is not for a {0} file. Upload a file that uses the namespace '{1}'.";
        private const string IncorrectNamespaceMessageTemplate = "The XML file you have provided is for {0}. You must provide a {1} XML file.";

        private readonly Dictionary<string, string> validNamespaceDictionary;
         
        public NamespaceValidator()
        {
            validNamespaceDictionary = new Dictionary<string, string>();

            validNamespaceDictionary.Add(XmlNamespace.MemberRegistration, "member registration");
            validNamespaceDictionary.Add(XmlNamespace.DataReturns, "data returns");
        }

        public IEnumerable<XmlValidationError> Validate(string expectedNamespace, string actualNamespace)
        {
            var errors = new List<XmlValidationError>();

            if (!validNamespaceDictionary.ContainsKey(expectedNamespace))
            {
                throw new ArgumentException(string.Format("The expected namespace '{0}' is not a namespace recognised by the system", expectedNamespace));
            } 

            if (actualNamespace != expectedNamespace)
            {
                if (validNamespaceDictionary.ContainsKey(actualNamespace))
                {
                    var message = string.Format(IncorrectNamespaceMessageTemplate,
                        validNamespaceDictionary[actualNamespace], validNamespaceDictionary[expectedNamespace]);

                    errors.Add(new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, message));
                }
                else
                {
                    var message = string.Format(InvalidNamespaceMessageTemplate,
                        validNamespaceDictionary[expectedNamespace], expectedNamespace);

                    errors.Add(new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, message));
                }
            }

            return errors;
        }
    }
}
