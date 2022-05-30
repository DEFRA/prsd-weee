namespace EA.Weee.Core.Validation
{
    using System;

    public class TonnageValidationResult : Exception
    {
        public TonnageValidationTypeEnum Type { get; private set; }

        public static readonly TonnageValidationResult Success;

        public TonnageValidationResult(TonnageValidationTypeEnum type)
        {
            Type = type;
        }
    }
}
