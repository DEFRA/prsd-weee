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
                string pattern = @"^\+?[0-9\s\-\(\)]{7,20}$";
                return Regex.IsMatch(faxNumber, pattern);
            }

            return false;
        }
    }
}
