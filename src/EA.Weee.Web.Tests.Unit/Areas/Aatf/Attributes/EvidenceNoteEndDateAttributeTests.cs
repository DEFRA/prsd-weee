namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Services.Caching;
    using Web.Areas.Aatf.Attributes;
    using Xunit;

    public class EvidenceNoteEndDateAttributeTests
    {
        private readonly IWeeeCache cache;
        private readonly EvidenceNoteEndDateAttribute attribute;

        public EvidenceNoteEndDateAttributeTests()
        {
            cache = A.Fake<IWeeeCache>();

            attribute = new EvidenceNoteEndDateAttribute("StartDate")
            {
                Cache = cache
            };

            A.CallTo(() => cache.FetchCurrentDate()).Returns(SystemTime.Now);
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteEndDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_CurrentDateShouldBeRetrievedFromCache()
        {
            //arrange
            var target = new ValidationTarget() { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
            var context = new ValidationContext(target);

            //act
            attribute.Validate(target.EndDate, context);

            //assert
            A.CallTo(() => cache.FetchCurrentDate()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateDateIsBeforeStartDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate.AddDays(-1) };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Ensure the end date is after the start date");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsAfterCurrentComplianceYear_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);
            var outOfComplianceYear = new DateTime(SystemTime.Now.Year + 1, 1, 1);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = outOfComplianceYear };
            var context = new ValidationContext(target);
            A.CallTo(() => cache.FetchCurrentDate()).Returns(SystemTime.Now);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("The end date must be within the current compliance year");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenStartDateIsEmpty_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);
            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);
            var target = new ValidationTarget() { StartDate = DateTime.MinValue, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenStartDateIsNull_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = null, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        private class ValidationTarget
        {
            public DateTime? StartDate { get; set; }

            [EvidenceNoteEndDate(nameof(StartDate), true)]
            public DateTime? EndDate { get; set; }
        }
    }
}
