namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericFaxNumberAttribute : ValidationAttribute
    {
        public bool AllowNull { get; set; }
        public GenericFaxNumberAttribute(bool allowNull = true)
        {
            AllowNull = allowNull;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return AllowNull;
            }

            if (value is string faxNumber)
            {
                Regex validRegex = new Regex(@"^[0-9+.()\s-]+$");
                return validRegex.IsMatch(faxNumber);
            }

            return false;
        }
    }
}
