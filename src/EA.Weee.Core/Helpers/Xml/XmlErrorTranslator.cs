namespace EA.Weee.Core.Helpers.Xml
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class XmlErrorTranslator : IXmlErrorTranslator
    {
        private const string GeneralConstraintFailurePattern =
            @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The ([^']*) constraint failed.$";

        private const string LengthConstraintFailurePattern =
            @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The actual length is (less|greater) than the (MinLength|MaxLength) value.$";

        private const string InvalidChildElementPattern =
            @"^The element '([^']*)' in namespace '[^']*' has invalid child element '([^']*)' in namespace '[^']*'. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

        private const string ErrorInXmlDocumentPattern = @"^There is an error in XML document \(([0-9]*)\,\s([0-9]*)\)\.$";

        public string MakeFriendlyErrorMessage(string message)
        {
            return MakeFriendlyErrorMessage(null, message, -1);
        }

        public string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber)
        {
            string resultErrorMessage = message;

            if (Regex.IsMatch(message, GeneralConstraintFailurePattern))
            {
                resultErrorMessage = MakeFriendlyGeneralConstraintFailureMessage(sender, message);
            }
            else if (Regex.IsMatch(message, LengthConstraintFailurePattern))
            {
                resultErrorMessage = MakeFriendlyLengthConstraintFailureMessage(sender, message);
            }
            else if (Regex.IsMatch(message, InvalidChildElementPattern))
            {
                resultErrorMessage = MakeFriendlyInvalidChildElementMessage(sender, message);
            }
            else if (Regex.IsMatch(message, ErrorInXmlDocumentPattern))
            {
                resultErrorMessage = MakeFriendlyErrorInXmlDocumentMessage(message);
            }

            var registrationNo = GetRegistrationNumber(sender);
            var registrationNoText = !string.IsNullOrEmpty(registrationNo) ? string.Format("Producer {0}: ", registrationNo) : string.Empty;

            var lineNumberText = lineNumber > 0 ? string.Format(" (Line {0}.)", lineNumber) : string.Empty;

            return string.Format("{0}{1}{2}", registrationNoText, resultErrorMessage, lineNumberText);
        }

        private string MakeFriendlyGeneralConstraintFailureMessage(XElement sender, string exceptionMessage)
        {
            var constraintWhichFailed = Regex.Match(exceptionMessage, GeneralConstraintFailurePattern).Groups[1].ToString();

            string friendlyMessageTemplate = string.Empty;

            switch (constraintWhichFailed)
            {
                case "MinInclusive":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' is too low.";
                    break;
                case "MaxInclusive":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' is too high.";
                    break;
                case "Pattern":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' doesn't match the required format.";
                    break;
                case "Enumeration":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' isn't one of the accepted values.";
                    break;
            }

            if (friendlyMessageTemplate != string.Empty)
            {
                return string.Format(friendlyMessageTemplate, sender.Value, sender.Name.LocalName);
            }

            return exceptionMessage;
        }

        private string MakeFriendlyLengthConstraintFailureMessage(XElement sender, string exceptionMessage)
        {
            var lengthConstraintWhichFailed = Regex.Match(exceptionMessage, LengthConstraintFailurePattern).Groups[2].ToString();

            string friendlyMessageTemplate = string.Empty;

            switch (lengthConstraintWhichFailed)
            {
                case "MinLength":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' is too short.";
                    break;
                case "MaxLength":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' is too long.";
                    break;
            }

            if (friendlyMessageTemplate != string.Empty)
            {
                return string.Format(friendlyMessageTemplate, sender.Value, sender.Name.LocalName);
            }

            return exceptionMessage;
        }

        private string MakeFriendlyInvalidChildElementMessage(XElement sender, string message)
        {
            return string.Format("The field {0} isn't expected here.", sender.Name.LocalName);
        }

        private string MakeFriendlyErrorInXmlDocumentMessage(string message)
        {
            var lineNumber = Regex.Match(message, ErrorInXmlDocumentPattern).Groups[1].ToString();

            return string.Format("{0} This can be caused by an error on this line, or before it. (Line {1}.)", message, lineNumber);
        }

        private string GetRegistrationNumber(XElement sender)
        {
            if (sender == null)
            {
                return string.Empty;
            }

            var associatedProducerElement =
                sender.AncestorsAndSelf().FirstOrDefault(e => e.Name.LocalName == "producer");

            if (associatedProducerElement == null)
            {
                return string.Empty;
            }

            var registrationNoElement =
                associatedProducerElement.Elements().FirstOrDefault(e => e.Name.LocalName == "registrationNo");

            return registrationNoElement != null ? registrationNoElement.Value : string.Empty;
        }
    }
}
