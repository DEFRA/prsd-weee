namespace EA.Weee.Core.Helpers
{
    using System;
    using System.Web.Routing;

    public class RouteValueBuilder
    {
        private readonly RouteValueDictionary values = new RouteValueDictionary();

        public RouteValueBuilder Add(string key, object value)
        {
            values[key] = value;
            return this;
        }

        public RouteValueBuilder AddIf(bool condition, string key, object value)
        {
            if (condition)
            {
                values[key] = value;
            }
            return this;
        }

        public RouteValueBuilder AddIfNotEmpty(string value, string key)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                values[key] = value;
            }
            return this;
        }

        public RouteValueBuilder AddIfHasValue<T>(T? value, string key, Func<T, object> transform = null) where T : struct
        {
            if (value.HasValue)
            {
                values[key] = transform != null ? transform(value.Value) : value.Value;
            }
            return this;
        }

        public RouteValueDictionary Build() => values;
    }
}
