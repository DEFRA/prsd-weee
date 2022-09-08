namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of a KeyValuePair with a parameterless constructor and public setters.
    /// This allows the MVC model binding to bind correctly unlike the KeyValuePair type.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public class GenericControlPair<TKey, TValue>
    {
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public GenericControlPair()
        {
        }

        /// <summary>
        /// Generate a radio button pair from a key and value.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The value to use.</param>
        public GenericControlPair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Generate a radio button pair directly from a KeyValuePair of the correct type.
        /// </summary>
        /// <param name="keyValuePair">The KeyValuePair to use.</param>
        public GenericControlPair(KeyValuePair<TKey, TValue> keyValuePair)
        {
            this.Key = keyValuePair.Key;
            this.Value = keyValuePair.Value;
        }

        /// <summary>
        /// The key displayed next to the radio button.
        /// </summary>
        public virtual TKey Key { get; set; }

        /// <summary>
        /// The value of the radio button when selected.
        /// </summary>
        public virtual TValue Value { get; set; }

        /// <summary>
        /// Provides explicit conversion to a KeyValuePair of the correct type.
        /// </summary>
        /// <param name="rbp">The GenericControlPair to convert from.</param>
        /// <returns>A KeyValuePair of the correct type.</returns>
        public static explicit operator KeyValuePair<TKey, TValue>(GenericControlPair<TKey, TValue> rbp)
        {
            if (rbp.Key == null || rbp.Value == null)
            {
                throw new InvalidOperationException("Attempted to cast from an empty GenericControlPair.");
            }
            return new KeyValuePair<TKey, TValue>(rbp.Key, rbp.Value);
        }

        /// <summary>
        /// Provides implicit conversion to a GenericControlPair of the same type as the KeyValuePair.
        /// </summary>
        /// <param name="kvp">The KeyValuePair to convert from.</param>
        /// <returns>A GenericControlPair of the correct type.</returns>
        public static implicit operator GenericControlPair<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
        {
            return new GenericControlPair<TKey, TValue>(kvp);
        }
    }
}