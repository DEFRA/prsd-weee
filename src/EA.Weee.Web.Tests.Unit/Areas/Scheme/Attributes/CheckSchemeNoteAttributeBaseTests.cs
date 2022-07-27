namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Attributes
{
    using FluentAssertions;
    using Web.Areas.Scheme.Attributes;
    using Xunit;

    public class CheckSchemeNoteAttributeBaseTests
    {
        [Fact]
        public void CheckSchemeNoteAttributeBase_ShouldBeDerivedFromCheckNoteAttributeBase()
        {
            typeof(CheckSchemeNoteAttributeBase).Should().BeDerivedFrom<CheckNoteAttributeBase>();
        }

        [Fact]
        public void CheckSchemeNoteAttributeBase_ShouldBeAbstract()
        {
            typeof(CheckSchemeNoteAttributeBase).Should().BeAbstract();
        }
    }
}
