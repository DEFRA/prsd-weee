namespace EA.Weee.Core.Helpers
{
    using System;

    public static class ReflectionExtensions
    {
        public static string GetControllerName(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            var typeName = type.Name;

            const string controllerSuffix = "Controller";
            return typeName.EndsWith(controllerSuffix, StringComparison.Ordinal) ?
                typeName.Remove(typeName.Length - controllerSuffix.Length) : typeName;
        }
    }
}
