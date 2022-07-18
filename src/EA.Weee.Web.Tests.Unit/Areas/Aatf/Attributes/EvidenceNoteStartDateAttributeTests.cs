﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
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
        private readonly EvidenceNoteStartDateAttribute attributeWithNoComplianceYearCheck;

        public EvidenceNoteStartDateAttributeTests()
        {
            cache = A.Fake<IWeeeCache>();

            attribute = new EvidenceNoteStartDateAttribute("EndDate", true)
            {
                Cache = cache
            };

            attributeWithNoComplianceYearCheck = new EvidenceNoteStartDateAttribute("EndDate", false)
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
            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
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
            var currentDate = new DateTime(2020, 1, 1);
            SystemTime.Freeze(currentDate);
            var target = new ValidationTargetWithComplianceYearCheck() {StartDate = currentDate.AddDays(1), EndDate = currentDate.AddDays(2) };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date cannot be in the future. Select today's date or earlier.");
            SystemTime.Unfreeze();
        }

        public static IEnumerable<object[]> ValidStartDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 31), new DateTime(2020, 1, 31) },
                new object[] { new DateTime(2020, 1, 31), new DateTime(2020, 1, 1) },
                new object[] { new DateTime(2020, 1, 1), new DateTime(2019, 12, 31) },
            };

        [Theory]
        [MemberData(nameof(ValidStartDates))]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsInAllowedComplianceYear_ValidationExceptionShouldNotBeThrown(DateTime currentDate, DateTime startDate)
        {
            //arrange
            SystemTime.Freeze(currentDate);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = startDate, EndDate = startDate.AddDays(1) };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        public static IEnumerable<object[]> InValidStartDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2019, 12, 31) },
                new object[] { new DateTime(2018, 12, 31) }
            };

        [Theory]
        [MemberData(nameof(InValidStartDates))]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsBeforeCurrentComplianceYear_ValidationExceptionShouldBeThrown(DateTime date)
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            SystemTime.Freeze(currentDate);
            
            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = date, EndDate = date.AddDays(1) };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date must be within the current compliance year");

            SystemTime.Unfreeze();
        }

        [Theory]
        [MemberData(nameof(InValidStartDates))]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsBeforeCurrentComplianceYearAndCheckComplianceYearIsFalse_ValidationExceptionShouldNotBeThrown(DateTime date)
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTargetWithoutComplianceYearCheck() { StartDate = date, EndDate = date.AddDays(1) };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

            //act
            var result = Record.Exception(() => attributeWithNoComplianceYearCheck.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterEndDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate.AddDays(-1), EndDate = currentDate.AddDays(-2) };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

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
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

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
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate, EndDate = DateTime.MinValue };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

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
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate, EndDate = null };
            var context = new ValidationContext(target);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        private class ValidationTargetWithComplianceYearCheck
        {
            [EvidenceNoteStartDate(nameof(EndDate), true)]
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }

        private class ValidationTargetWithoutComplianceYearCheck
        {
            [EvidenceNoteStartDate(nameof(EndDate), false)]
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }
    }
}
