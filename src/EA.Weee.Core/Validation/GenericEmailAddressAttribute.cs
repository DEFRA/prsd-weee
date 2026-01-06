namespace EA.Weee.Core.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericEmailAddressAttribute : ValidationAttribute
    {
        public bool AllowNull { get; set; }
        public GenericEmailAddressAttribute(bool allowNull = false)
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

            if (value is string email)
            {
                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }

            return false;
        }
    }
}
