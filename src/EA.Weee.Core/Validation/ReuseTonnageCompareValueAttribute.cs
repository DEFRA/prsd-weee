namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.DataReturns;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web.Mvc;

    public class ReuseTonnageCompareValueAttribute : ValidationAttribute
    {
        public string TransferReceievedName { get; private set; }
        public string AvailableRecievedName { get; private set; }
        public string TransferReusedName { get; private set; }
        public string AvailableReusedName { get; private set; }
        public string CategoryProperty { get; private set; }

        private ITonnageValueValidator tonnageValueValidator;
        public ITonnageValueValidator TonnageValueValidator
        {
            get => tonnageValueValidator ?? DependencyResolver.Current.GetService<ITonnageValueValidator>();
            set => tonnageValueValidator = value;
        }

        public ReuseTonnageCompareValueAttribute(string category,
            string transferReceievedName,
            string availableRecievedName,
            string transferResusedName, string availableReusedName, string errorMessage)
        {
            TransferReceievedName = transferReceievedName;
            AvailableRecievedName = availableRecievedName;
            TransferReusedName = transferResusedName;
            AvailableReusedName = availableReusedName;
            CategoryProperty = category;
            ErrorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var categoryProperty = type.GetProperty(CategoryProperty);
            var categoryPropertyValue = (int)categoryProperty.GetValue(validationContext.ObjectInstance, null) as int?;
            var availableReceivedProperty = type.GetProperty(AvailableRecievedName);
            var availableReceivedValue = availableReceivedProperty.GetValue(instance, null);
            var transferRecievedProperty = type.GetProperty(TransferReceievedName);
            var transferRecievedValue = transferRecievedProperty.GetValue(instance, null);
            var availableReusedProperty = type.GetProperty(AvailableReusedName);
            var availableReusedValue = availableReusedProperty.GetValue(instance, null);
            var transferReusedProperty = type.GetProperty(TransferReusedName);
            var transferReusedValue = transferReusedProperty.GetValue(instance, null);

            decimal.TryParse(availableReceivedValue != null ? availableReceivedValue.ToString() : "0", NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                                        CultureInfo.InvariantCulture, out var decimalAvailableReceivedResult);
            decimal.TryParse(transferRecievedValue != null ? transferRecievedValue.ToString() : "0", NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                            CultureInfo.InvariantCulture, out var decimalTransferReceivedResult);
            decimal.TryParse(availableReusedValue != null ? availableReusedValue.ToString() : "0", NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                            CultureInfo.InvariantCulture, out var decimalAvailableReusedResult);
            decimal.TryParse(transferReusedValue != null ? transferReusedValue.ToString() : "0", NumberStyles.Number & ~NumberStyles.AllowTrailingSign,
                            CultureInfo.InvariantCulture, out var decimalTransferReusedResult);

            if (((decimalAvailableReceivedResult - decimalTransferReceivedResult) - (decimalAvailableReusedResult - decimalTransferReusedResult)) < 0)
            {
                return new ValidationResult(GenerateMessage(categoryPropertyValue.Value));
            }

            return ValidationResult.Success;
        }

        private string GenerateMessage(int categoryId)
        {
            var category = $"{categoryId} {((WeeeCategory)categoryId).ToCustomDisplayString()}";

            return string.Format(ErrorMessage, category);
        }
    }
}
