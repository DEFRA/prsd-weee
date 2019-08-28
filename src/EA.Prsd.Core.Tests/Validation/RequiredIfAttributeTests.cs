namespace EA.Prsd.Core.Tests.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;
    using Xunit;

    public class RequiredIfAttributeTests
    {
        private static bool IsValid(TestObject testObject)
        {
            var resultList = new List<ValidationResult>();
            return IsValid(testObject, resultList);
        }

        private static bool IsValid(TestObject testObject, List<ValidationResult> resultList)
        {
            return Validator.TryValidateProperty(testObject.SomeProperty, new ValidationContext(testObject) { MemberName = "SomeProperty" }, resultList);
        }

        [Fact]
        public void SomePropertyIsNull_AnotherPropertyIsDesiredValue_IsNotValid()
        {
            var testObject = new TestObject
            {
                AnotherProperty = "Required"
            };

            Assert.False(IsValid(testObject));
        }

        [Fact]
        public void SomePropertyIsNotNull_AnotherPropertyIsDesiredValue_IsValid()
        {
            var testObject = new TestObject
            {
                SomeProperty = "Something",
                AnotherProperty = "Required"
            };

            Assert.True(IsValid(testObject));
        }

        [Fact]
        public void ErrorMessageSetFromResource()
        {
            var testObject = new TestObject
            {
                AnotherProperty = "Required"
            };

            var resultList = new List<ValidationResult>();
            IsValid(testObject, resultList);

            Assert.Equal(TestResources.ErrorMessage, resultList[0].ErrorMessage);
        }

        private class TestObject
        {
            [RequiredIf("AnotherProperty", "Required", ErrorMessageResourceName = "ErrorMessage", ErrorMessageResourceType = typeof(TestResources))]
            public string SomeProperty { get; set; }

            public string AnotherProperty { get; set; }
        }
    }
}