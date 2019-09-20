namespace EA.Prsd.Core.Tests.Helpers
{
    using Core.Helpers;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class EnumHelperTests
    {
        private enum TestEnum
        {
            [Display(Name = "Barker")]
            [Description("Likes to bark all day.")]
            Dog = 1,

            [Display(Name = "Puss-in-boots", ShortName = "Puss", Description = "A cat by day, daring international criminal by night")]
            Cat = 2,
            
            Parrot = 3,

            [Display(Name = "Fantastic Mr Fox")]
            Fox = 4,

            [Display(Name = "Chicken Little", Description = "A chicken")]
            [Description("A cowardly chicken")]
            Chicken = 5
        }

        [Fact]
        public void GetShortName()
        {
            Assert.Equal("Puss", EnumHelper.GetShortName(TestEnum.Cat));
        }

        [Fact]
        public void GetDisplayName()
        {
            Assert.Equal("Barker", EnumHelper.GetDisplayName(TestEnum.Dog));
        }

        [Fact]
        public void GetDescription()
        {
            Assert.Equal("Likes to bark all day.", EnumHelper.GetDescription(TestEnum.Dog));
        }

        [Fact]
        public void GetDescription_UsesDisplayAttribute_WhenDescriptionNotPresent()
        {
            Assert.Equal("A cat by day, daring international criminal by night", EnumHelper.GetDescription(TestEnum.Cat));
        }

        [Fact]
        public void GetDescription_UsesEnumValue_WhenDescriptionNotOnAttribute()
        {
            Assert.Equal("Fox", EnumHelper.GetDescription(TestEnum.Fox));
        }

        [Fact]
        public void GetDescription_DefaultsToEnumValue()
        {
            Assert.Equal("Parrot", EnumHelper.GetDescription(TestEnum.Parrot));
        }

        [Fact]
        public void GetDescription_UsesDescriptionAttributeFirst()
        {
            Assert.Equal("A cowardly chicken", EnumHelper.GetDescription(TestEnum.Chicken));
        }

        [Fact]
        public void GetShortName_DefaultsToEnumValue()
        {
            Assert.Equal("Parrot", EnumHelper.GetShortName(TestEnum.Parrot));
        }

        [Fact]
        public void GetDisplayName_DefaultsToEnumValue()
        {
            Assert.Equal("Parrot", EnumHelper.GetDisplayName(TestEnum.Parrot));
        }
    }
}
