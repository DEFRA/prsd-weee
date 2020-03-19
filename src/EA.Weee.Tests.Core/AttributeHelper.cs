namespace EA.Weee.Tests.Core
{
    using System;
    using System.Web.Http;
    using FluentAssertions;

    public static class AttributeHelper
    {
        public static void ShouldNotHaveAnonymousMethods(Type type)
        {
            foreach (var method in type.GetMethods())
            {
                method.Should().NotBeDecoratedWith<AllowAnonymousAttribute>();
            }
        }
    }
}
