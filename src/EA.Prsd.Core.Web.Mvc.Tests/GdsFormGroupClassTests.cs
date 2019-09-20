namespace EA.Prsd.Core.Web.Mvc.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using Helpers;
    using ViewModels;
    using Xunit;

    public class GdsFormGroupClassTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>(); 

        [Fact]
        public void SinglePropertyReturnsName()
        {
            var propertyName = GetPropertyName(m => m.TopLevel);

            Assert.Equal("TopLevel", propertyName);
        }

        [Fact]
        public void SingleClassPropertyReturnsName()
        {
            var propertyName = GetPropertyName(m => m.Nested);

            Assert.Equal("Nested", propertyName);
        }

        [Fact]
        public void NestedPropertyReturnsName()
        {
            var propertyName = GetPropertyName(m => m.Nested.MiddleLevel);

            Assert.Equal("Nested.MiddleLevel", propertyName);
        }

        [Fact]
        public void DeepNestedPropertyReturnsName()
        {
            var propertyName = GetPropertyName(m => m.Nested.Bottom.BottomLevel);

            Assert.Equal("Nested.Bottom.BottomLevel", propertyName);
        }

        [Fact]
        public void SecondDeepNestedPropertyReturnsName()
        {
            var propertyName = GetPropertyName(m => m.Nested.Bottom.Depth);

            Assert.Equal("Nested.Bottom.Depth", propertyName);
        }

        private string GetPropertyName<TProperty>(Expression<Func<TestModel, TProperty>> expression)
        {
            return GdsExtensionTester<TestModel>.GetPropertyName(htmlHelper, expression);
        }
    }
}
