namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Services.Caching;
    using Web.Areas.Aatf.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNoteStartDateAttributeTests
    {
        private readonly List<ValidationResult> validationResults;
        private readonly IWeeeCache cache;

        public EvidenceNoteStartDateAttributeTests()
        {
            validationResults = new List<ValidationResult>();
            cache = A.Fake<IWeeeCache>();

            var container = new MockDependencyResolver();
            container.Freeze<IWeeeCache>(cache);
            DependencyResolver.SetResolver(container);

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
            var attr = new EvidenceNoteStartDateAttribute("EndDate");

            //act
            attr.Validate(target.StartDate, context);

            //assert
            A.CallTo(() => cache.FetchCurrentDate()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterToday_FalseShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;

            SystemTime.Freeze(currentDate);
            var target = new ValidationTarget() {StartDate = currentDate.AddDays(1), EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(
                    "The start date cannot be in the future. Select today's date or earlier.")
            });

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsBeforeCurrentComplianceYear_FalseShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);
            var outOfComplianceYear = new DateTime(SystemTime.Now.Year, 1, 1).AddMilliseconds(-1);
            var target = new ValidationTarget() { StartDate = outOfComplianceYear, EndDate = currentDate };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(SystemTime.Now);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(
                    "The start date must be within the current compliance year")
            });

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterEndDate_FalseShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate.AddDays(-1), EndDate = currentDate.AddDays(-2) };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(
                    "Ensure the start date is before the end date")
            });

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsEqualToTheEndDate_TrueShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenEndDateIsEmpty_TrueShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = DateTime.MinValue };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenEndDateIsNull_TrueShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = null };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();

            SystemTime.Unfreeze();
        }

        private class ValidationTarget
        {
            [EvidenceNoteStartDate(nameof(EndDate))]
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }
    }
}
