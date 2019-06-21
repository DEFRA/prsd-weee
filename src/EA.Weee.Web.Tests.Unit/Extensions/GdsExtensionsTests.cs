namespace EA.Weee.Web.Tests.Unit.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Prsd.Core.Web.Mvc.RazorHelpers;
    using EA.Weee.Web.Tests.Unit.TestHelpers.Classes;
    using FakeItEasy;
    using Web.Extensions;
    using Xunit;

    public class GdsExtensionsTests
    {
        private Gds<TestViewModelBase> gds;

        public GdsExtensionsTests()
        {
            gds = A.Fake<Gds<TestViewModelBase>>();
        }

        [Fact]
        public void LabelForOverridden_NoBaseDisplayAttribute_NotOveridden_ReturnsUnmodifiedString()
        {
            var unmodifiedString = gds.LabelFor(m => m.NoBaseDisplayAttribute, false);

            var result = gds.LabelForOverridden(m => m.NoBaseDisplayAttribute, typeof(TestViewModel));

            Assert.Equal(unmodifiedString.ToString(), result.ToString());
        }

        [Fact]
        public void LabelForOverridden_NoBaseDisplayAttribute_Overidden_ReturnsModifiedString()
        {
            var descriptionAttributes = (DisplayAttribute[])typeof(TestViewModel)
                .GetProperty(nameof(TestViewModel.NoBaseDisplayAttributeOverridden))
                .GetCustomAttributes(typeof(DisplayAttribute), false);
            var expectedDisplayString = descriptionAttributes[0].Name;

            var result = gds.LabelForOverridden(m => m.NoBaseDisplayAttributeOverridden, typeof(TestViewModel));

            Assert.Contains($">{expectedDisplayString}</label>", result.ToString());
        }

        [Fact]
        public void LabelForOverridden_WithBaseDisplayAttribute_NotOveridden_ReturnsUnmodifiedString()
        {
            var unmodifiedString = gds.LabelFor(m => m.WithBaseDisplayAttribute, false);

            var result = gds.LabelForOverridden(m => m.WithBaseDisplayAttribute, typeof(TestViewModel));

            Assert.Equal(unmodifiedString.ToString(), result.ToString());
        }

        [Fact]
        public void LabelForOverridden_WithBaseDisplayAttribute_Overidden_ReturnsModifiedString()
        {
            var descriptionAttributes = (DisplayAttribute[])typeof(TestViewModel)
                .GetProperty(nameof(TestViewModel.WithBaseDisplayAttributeOverridden))
                .GetCustomAttributes(typeof(DisplayAttribute), false);
            var expectedDisplayString = descriptionAttributes[0].Name;

            var result = gds.LabelForOverridden(m => m.WithBaseDisplayAttributeOverridden, typeof(TestViewModel));

            Assert.Contains($">{expectedDisplayString}</label>", result.ToString());
        }

        [Fact]
        public void LabelForOverridden_NotABaseType_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() => gds.LabelForOverridden(m => m.WithBaseDisplayAttribute, typeof(string)));
            Assert.Contains(typeof(TestViewModelBase).ToString(), exception.Message);
            Assert.Contains(typeof(string).ToString(), exception.Message);
        }
    }
}
