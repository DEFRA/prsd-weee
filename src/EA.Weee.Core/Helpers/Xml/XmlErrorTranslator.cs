namespace EA.Weee.Core.Helpers.Xml
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class XmlErrorTranslator : IXmlErrorTranslator
    {
        private const string DataAtTheRoolLevelIsInvalid = @"^Data at the root level is invalid\..*$";

        private const string GeneralConstraintFailurePattern =
            @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The ([^']*) constraint failed.$";

        private const string LengthConstraintFailurePattern =
            @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The actual length is (less|greater) than the (MinLength|MaxLength) value.$";

        private const string InvalidChildElementPattern =
            @"^The element '([^']*)' in namespace '[^']*' has invalid child element '([^']*)' in namespace '[^']*'. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

        private const string IncompleteContentPattern =
           @"^The element '([^']*)' in namespace '[^']*' has incomplete content. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

        private const string ErrorInXmlDocumentPattern = @"^There is an error in XML document \(([0-9]*)\,\s([0-9]*)\)\.$";

        public string MakeFriendlyErrorMessage(string message)
        {
            return MakeFriendlyErrorMessage(null, message, -1);
        }

        public string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber)
        {
            string resultErrorMessage = message;

            if (Regex.IsMatch(message, DataAtTheRoolLevelIsInvalid))
            {
                resultErrorMessage = "The file you're trying to upload is not a correctly formatted XML file. Please make sure you're uploading a valid XML file.";
            }
            else if (Regex.IsMatch(message, GeneralConstraintFailurePattern))
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
            else if (Regex.IsMatch(message, IncompleteContentPattern))
            {
                resultErrorMessage = MakeFriendlyIncompleteContentMessage(sender, message);
            }
            else if (Regex.IsMatch(message, ErrorInXmlDocumentPattern))
            {
                resultErrorMessage = MakeFriendlyErrorInXmlDocumentMessage(message);
            }

            var registrationNo = GetRegistrationNumber(sender);
            var registrationNoText = !string.IsNullOrEmpty(registrationNo) ? string.Format("Producer {0}: ", registrationNo) : string.Empty;

            var lineNumberText = lineNumber > 0 ? string.Format(" (XML line {0})", lineNumber) : string.Empty;

            return string.Format("{0}{1}{2}", registrationNoText, resultErrorMessage, lineNumberText);
        }

        private string MakeFriendlyGeneralConstraintFailureMessage(XElement sender, string exceptionMessage)
        {
            var constraintWhichFailed = Regex.Match(exceptionMessage, GeneralConstraintFailurePattern).Groups[1].ToString();

            string friendlyMessageTemplate = string.Empty;

            switch (constraintWhichFailed)
            {
                case "MinInclusive":
                case "MaxInclusive":
                    friendlyMessageTemplate =
                        "The {1} you've provided is incorrect. Please make sure you enter the right value."; 
                    break;
                case "Pattern":
                    friendlyMessageTemplate = string.IsNullOrEmpty(sender.Value) ? "Mandatory {1} field." : "The value '{0}' supplied for field '{1}' doesn't match the required format.";
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

        private string MakeFriendlyIncompleteContentMessage(XElement sender, string message)
        {
            string listName = sender.Name.LocalName;
            if (sender.Name.LocalName.Contains("List"))
            {
                listName = listName.Substring(0, listName.Length - 4);
            }
            return string.Format("There are no {0} details in XML file. Plesae provide details for at least one {0}.", listName);    
        }

        private string MakeFriendlyErrorInXmlDocumentMessage(string message)
        {
            var lineNumber = Regex.Match(message, ErrorInXmlDocumentPattern).Groups[1].ToString();

            return string.Format("{0} This can be caused by an error on this line, or before it. (XML line {1})", message, lineNumber);
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
