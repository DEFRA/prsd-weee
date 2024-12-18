namespace EA.Prsd.Core.Tests.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;
    using Xunit;

    public class RequiredIfOtherValueNotNullAttributeTest
    {
        private static bool SomePropertyIsValid(TestObject testObject)
        {
            var resultList = new List<ValidationResult>();

            var res = SomePropertyIsValid(testObject, resultList);

            return res;
        }

        private static bool AnotherPropertyIsValid(TestObject testObject)
        {
            var resultList = new List<ValidationResult>();

            var res = AnotherPropertyIsValid(testObject, resultList);

            return res;
        }

        private static bool SomePropertyIsValid(TestObject testObject, List<ValidationResult> resultList)
        {
            return Validator.TryValidateProperty(testObject.SomeProperty, new ValidationContext(testObject) { MemberName = "SomeProperty" }, resultList);
        }

        private static bool AnotherPropertyIsValid(TestObject testObject, List<ValidationResult> resultList)
        {
            return Validator.TryValidateProperty(testObject.AnotherProperty, new ValidationContext(testObject) { MemberName = "AnotherProperty" }, resultList);
        }

        [Fact]
        public void AnotherPropertyIsNull_SomePropertyIsNotNull_IsNotValid()
        {
            var testObject = new TestObject
            {
                AnotherProperty = null, 
                SomeProperty = "something"
            };

            Assert.False(AnotherPropertyIsValid(testObject));
        }

        [Fact]
        public void AnotherPropertyIsNotNull_SomePropertyIsNull_IsNotValid()
        {
            var testObject = new TestObject
            {
                AnotherProperty = "value",
                SomeProperty = null
            };

            Assert.False(SomePropertyIsValid(testObject));
        }

        [Fact]
        public void AnotherPropertyIsNull_SomePropertyNull_IsValid()
        {
            var testObject = new TestObject
            {
                AnotherProperty = null,
                SomeProperty = null
            };

            Assert.True(SomePropertyIsValid(testObject));
            Assert.True(AnotherPropertyIsValid(testObject));
        }

        [Fact]
        public void ErrorMessageSetFromResource()
        {
            var testObject = new TestObject
            {
                AnotherProperty = "Required"
            };

            var resultList = new List<ValidationResult>();
            SomePropertyIsValid(testObject, resultList);

            Assert.Equal(TestResources.ErrorMessage, resultList[0].ErrorMessage);
        }

        private class TestObject
        {
            [RequiredIfOtherValueNotNull("AnotherProperty", ErrorMessageResourceName = "ErrorMessage", ErrorMessageResourceType = typeof(TestResources))]
            public string SomeProperty { get; set; }

            [RequiredIfOtherValueNotNull("SomeProperty", ErrorMessageResourceName = "ErrorMessage", ErrorMessageResourceType = typeof(TestResources))]
            public string AnotherProperty { get; set; }
        }
    }
}