namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    internal class XmlErrorTranslator : IXmlErrorTranslator
    {
        private const string GeneralConstraintFailurePattern =
            @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The ([^']*) constraint failed.$";

        private const string LengthConstraintFailurePattern =
            @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The actual length is (less|greater) than the (MinLength|MaxLength) value.$";

        private const string InvalidChildElementPattern =
            @"^The element '([^']*)' in namespace '[^']*' has invalid child element '([^']*)' in namespace '[^']*'. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

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
                resultErrorMessage = "Invalid child element oh no! " + message;
            }

            return string.Format("{0} (Line {1}.)", resultErrorMessage, lineNumber);
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
    }
}
