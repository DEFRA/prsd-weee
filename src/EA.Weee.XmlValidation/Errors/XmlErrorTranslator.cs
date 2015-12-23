namespace EA.Weee.XmlValidation.Errors
{
    using System;
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

        private const string InvalidDataTypePattern =
          @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The string '[^']*' is not a valid (Boolean|Date|Decimal|Integer|Single) value.$";

        private const string InvalidChildElementPattern =
            @"^The element '([^']*)' in namespace '[^']*' has invalid child element '([^']*)' in namespace '[^']*'. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

        private const string IncompleteContentPattern =
           @"^The element '([^']*)' in namespace '[^']*' has incomplete content. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

        private static readonly Regex FixedValueConstraintFailurePattern = new Regex(
           @"^The value of the '(?<ElementName>[^']*)' element does not equal its fixed value.$",
           RegexOptions.Compiled);

        private static readonly Regex EntityNameParsingErrorPattern = new Regex(
            @"^An error occurred while parsing EntityName\. Line (?<LineNumber>\d+), position \d+\.$",
            RegexOptions.Compiled);

        private static readonly Regex UnterminatedCharacterEntityReferencePattern = new Regex(
            @"^'[^']*' is an unexpected token\. The expected token is ';'\. Line (?<LineNumber>\d+), position \d+\.$",
            RegexOptions.Compiled);

        private static readonly Regex InvalidCharacterEntityReferencePattern = new Regex(
            @"^Reference to undeclared entity '[^']*'\. Line (?<LineNumber>\d+), position \d+\.$",
            RegexOptions.Compiled);

        private const string ErrorInXmlDocumentPattern = @"^There is an error in XML document \(([0-9]*)\,\s([0-9]*)\)\.$";

        private const string UniqueKeyConstraintPattern = @"There is a duplicate key sequence '[^']*' for the '[^']*' key or unique identity constraint.$";

        public const string IncorrectlyFormattedXmlMessage =
            "The file you're trying to upload is not a correctly formatted XML file. Upload a valid XML file.";

        public string MakeFriendlyErrorMessage(string message, string schemaVersion)
        {
            return MakeFriendlyErrorMessage(null, message, -1, schemaVersion);
        }

        public string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber, string schemaVersion)
        {
            string resultErrorMessage = message;
            Match match = null;

            if (Regex.IsMatch(message, DataAtTheRoolLevelIsInvalid))
            {
                resultErrorMessage = IncorrectlyFormattedXmlMessage;
            }
            else if (Regex.IsMatch(message, GeneralConstraintFailurePattern))
            {
                resultErrorMessage = MakeFriendlyGeneralConstraintFailureMessage(sender, message);
            }
            else if (Regex.IsMatch(message, LengthConstraintFailurePattern))
            {
                resultErrorMessage = MakeFriendlyLengthConstraintFailureMessage(sender, message);
            }
            else if (Regex.IsMatch(message, InvalidDataTypePattern))
            {
                resultErrorMessage = MakeFriendlyInvalidDataTypeMessage(sender, message);
            }
            else if (Regex.IsMatch(message, InvalidChildElementPattern))
            {
                resultErrorMessage = MakeFriendlyInvalidChildElementMessage(sender, message, schemaVersion);
            }
            else if (Regex.IsMatch(message, IncompleteContentPattern))
            {
                resultErrorMessage = MakeFriendlyIncompleteContentMessage(sender, message);
            }
            else if (Regex.IsMatch(message, ErrorInXmlDocumentPattern))
            {
                resultErrorMessage = MakeFriendlyErrorInXmlDocumentMessage(message);
            }
            else if (TestRegex(message, EntityNameParsingErrorPattern, out match))
            {
                lineNumber = int.Parse(match.Groups["LineNumber"].Value);
                resultErrorMessage = "Your XML file is not encoded correctly. Check for any characters which need to be encoded. For example, replace ampersand (&) characters with &amp;.";
            }
            else if (TestRegex(message, UnterminatedCharacterEntityReferencePattern, out match))
            {
                lineNumber = int.Parse(match.Groups["LineNumber"].Value);
                resultErrorMessage = "Your XML file is not encoded correctly. Check for any characters which need to be encoded. For example, replace ampersand (&) characters with &amp;.";
            }
            else if (TestRegex(message, InvalidCharacterEntityReferencePattern, out match))
            {
                lineNumber = int.Parse(match.Groups["LineNumber"].Value);
                resultErrorMessage = "Your XML file is not encoded correctly. Check for any characters which need to be encoded. For example, replace ampersand (&) characters with &amp;.";
            }
            else if (Regex.IsMatch(message, UniqueKeyConstraintPattern))
            {
                resultErrorMessage = MakeFriendlyErrorUniqueKeyMessage(sender, message);
            }
            else if (TestRegex(message, FixedValueConstraintFailurePattern, out match))
            {
                resultErrorMessage = string.Format(
                    "The value '{0}' supplied for field '{1}' is not permitted. " +
                    "Only the value specified in the schema is allowed.",
                    sender.Value,
                    sender.Name.LocalName);
            }

            var registrationNo = GetRegistrationNumber(sender);
            var registrationNoText = !string.IsNullOrEmpty(registrationNo) ? string.Format("Producer {0}: ", registrationNo) : string.Empty;

            var lineNumberText = lineNumber > 0 ? string.Format(" (XML line {0}).", lineNumber) : string.Empty;

            if (!string.IsNullOrEmpty(lineNumberText))
            {
                resultErrorMessage = resultErrorMessage.EndsWith(".", StringComparison.CurrentCulture)
                    ? resultErrorMessage.Remove(resultErrorMessage.Length - 1)
                    : resultErrorMessage;
            }

            return string.Format("{0}{1}{2}", registrationNoText, resultErrorMessage, lineNumberText);
        }

        private string MakeFriendlyErrorUniqueKeyMessage(XElement sender, string message)
        {
            var element = sender.Descendants().First();
            return string.Format("There is duplicate value '{0}' for field '{1}' of parent field '{2}'. Remove one of the duplicate entries", element.Value, element.Name.LocalName, sender.Name.LocalName);
        }

        private static bool TestRegex(string message, Regex regex, out Match match)
        {
            match = regex.Match(message);
            return match.Success;
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
                        "The value '{0}' supplied for field '{1}' is lower than the minimum, or greater than the maximum, allowed value.";
                    break;
                case "Pattern":
                    friendlyMessageTemplate = string.IsNullOrEmpty(sender.Value) ? "You must provide a value for '{1}'." : "The value '{0}' supplied for field '{1}' doesn't match the required format.";
                    break;
                case "Enumeration":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' isn't one of the accepted values.";
                    break;
                case "FractionDigits":
                    friendlyMessageTemplate =
                        "The value '{0}' supplied for field '{1}' exceeds the maximum number of allowed decimal places";
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

        private string MakeFriendlyInvalidDataTypeMessage(XElement sender, string message)
        {
            var constraintWhichFailed = Regex.Match(message, InvalidDataTypePattern).Groups[1].ToString();

            string friendlyMessageTemplate = string.Empty;
            switch (constraintWhichFailed)
            {
                case "Integer":
                    friendlyMessageTemplate = "a whole number";
                    break;

                case "Boolean":
                    friendlyMessageTemplate = "true, false, 0 or 1";
                    break;

                case "Decimal":
                    friendlyMessageTemplate = "a decimal";
                    break;

                case "Date":
                    friendlyMessageTemplate = "a date in the format YYYY-MM-DD";
                    break;

                case "Single":
                    friendlyMessageTemplate = "a number";
                    break;
            }

            return
                string.Format(
                    "The value '{0}' supplied for field '{1}' doesn't match the required data type. The value '{0}' must be {2}.",
                    sender.Value, sender.Name.LocalName, friendlyMessageTemplate);
        }

        private string MakeFriendlyInvalidChildElementMessage(XElement sender, string message, string schemaVersion)
        {
            return string.Format("The field '{0}' isn't expected here. Check that you are using v{1} of the XSD schema (XML template).", sender.Name.LocalName, schemaVersion);
        }

        private string MakeFriendlyIncompleteContentMessage(XElement sender, string message)
        {
            string listName = sender.Name.LocalName;
            if (sender.Name.LocalName.Contains("List"))
            {
                listName = listName.Substring(0, listName.Length - 4);
            }

            var missingElement = message
                .Split(new[] { "List of possible elements expected:" }, StringSplitOptions.None)[1]
                .Split(new[] { "in namespace" }, StringSplitOptions.None)[0]
                .Trim();

            return string.Format("The '{0}' element is missing a child element {1}.", listName, missingElement);
        }

        private string MakeFriendlyErrorInXmlDocumentMessage(string message)
        {
            var lineNumber = Regex.Match(message, ErrorInXmlDocumentPattern).Groups[1].ToString();

            return string.Format("'{0}' This can be caused by an error on this line, or before it (XML line {1}).", message, lineNumber);
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
