namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Xunit;

    public class ControllerTests
    {
        // A really useful unit test that makes sure all your http post actions have the anti forgery token attribute
        // Stolen from http://codiply.com/blog/test-for-omission-of-validateantiforgerytoken-attribute-in-asp-net-mvc
        [Fact]
        public void AllHttpPostControllerActionsShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            var allControllerTypes =
                typeof(Global).Assembly.GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type));
            var allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods());

            var failingActions = allControllerActions
                .Where(method =>
                    Attribute.GetCustomAttribute(method, typeof(HttpPostAttribute)) != null)
                .Where(method =>
                    Attribute.GetCustomAttribute(method, typeof(ValidateAntiForgeryTokenAttribute)) == null)
                .ToList();

            var message = string.Empty;
            if (failingActions.Any())
            {
                message =
                    failingActions.Count() + " failing action" +
                    (failingActions.Count() == 1 ? ":\n" : "s:\n") +
                    failingActions.Select(method => method.Name + " in " + method.DeclaringType.Name)
                        .Aggregate((a, b) => a + ",\n" + b);
            }

            Assert.False(failingActions.Any(), message);
        }
    }
}
