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

    public class EvidenceNoteStartDateAttributeTests
    {
        private readonly IWeeeCache cache;
        private readonly EvidenceNoteStartDateAttribute attribute;

        public EvidenceNoteStartDateAttributeTests()
        {
            cache = A.Fake<IWeeeCache>();

            attribute = new EvidenceNoteStartDateAttribute("EndDate", true)
            {
                Cache = cache
            };

            A.CallTo(() => cache.FetchCurrentDate()).Returns(SystemTime.Now);
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteStartDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_CurrentDateShouldBeRetrievedFromCache()
        {
            //arrange
            var target = new ValidationTarget() { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
            var context = new ValidationContext(target);

            //act
            attribute.Validate(target.StartDate, context);

            //assert
            A.CallTo(() => cache.FetchCurrentDate()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterToday_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;

            SystemTime.Freeze(currentDate);
            var target = new ValidationTarget() {StartDate = currentDate.AddDays(1), EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date cannot be in the future. Select today's date or earlier.");
            
            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsBeforeCurrentComplianceYear_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);
            var outOfComplianceYear = new DateTime(SystemTime.Now.Year, 1, 1).AddMilliseconds(-1);
            var target = new ValidationTarget() { StartDate = outOfComplianceYear, EndDate = currentDate };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(SystemTime.Now);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date must be within the current compliance year");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterEndDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate.AddDays(-1), EndDate = currentDate.AddDays(-2) };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("Ensure the start date is before the end date");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsEqualToTheEndDate_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenEndDateIsEmpty_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = DateTime.MinValue };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenEndDateIsNull_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = null };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        private class ValidationTarget
        {
            [EvidenceNoteStartDate(nameof(EndDate), true)]
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }
    }
}
