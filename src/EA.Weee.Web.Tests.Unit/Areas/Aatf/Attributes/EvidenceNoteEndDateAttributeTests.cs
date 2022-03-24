﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Areas.Aatf.Attributes;
    using Xunit;

    public class EvidenceNoteEndDateAttributeTests
    {
        private readonly List<ValidationResult> validationResults;

        public EvidenceNoteEndDateAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteEndDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateDateIsBeforeStartDate_FalseShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate.AddDays(-1) };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(
                    "Ensure the end date is after the start date")
            });

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsAfterCurrentComplianceYear_FalseShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);
            var outOfComplianceYear = new DateTime(SystemTime.Now.Year + 1, 1, 1);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = outOfComplianceYear };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();
            validationResults.Should().BeEquivalentTo(new List<ValidationResult>()
            {
                new ValidationResult(
                    "The end date must be within the current compliance year")
            });

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenStartDateIsEmpty_TrueShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = DateTime.MinValue, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenStartDateIsNull_TrueShouldBeReturned()
        {
            //arrange
            var currentDate = DateTime.Now;
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = null, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();

            SystemTime.Unfreeze();
        }

        private class ValidationTarget
        {
            public DateTime? StartDate { get; set; }

            [EvidenceNoteEndDate(nameof(StartDate))]
            public DateTime? EndDate { get; set; }
        }
    }
}
