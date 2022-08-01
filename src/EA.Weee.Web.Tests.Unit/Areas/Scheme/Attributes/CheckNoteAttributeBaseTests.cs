namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Attributes
{
    using System.Web.Mvc;
    using FluentAssertions;
    using Web.Areas.Scheme.Attributes;
    using Xunit;

    public class CheckNoteAttributeBaseTests
    {
        [Fact]
        public void CheckNoteAttributeBase_ShouldBeDerivedFromCheckNoteAttributeBase()
        {
            typeof(CheckNoteAttributeBase).Should().BeDerivedFrom<ActionFilterAttribute>();
        }

        [Fact]
        public void CheckNoteAttributeBase_ShouldBeAbstract()
        {
            typeof(CheckNoteAttributeBase).Should().BeAbstract();
        }
    }
}
