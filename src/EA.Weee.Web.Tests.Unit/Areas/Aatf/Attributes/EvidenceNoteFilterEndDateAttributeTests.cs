namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Api.Client;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.Aatf.Attributes;
    using Xunit;

    public class EvidenceNoteFilterEndDateAttributeTests
    {
        private readonly IWeeeClient client;
        private readonly EvidenceNoteFilterEndDateAttribute attribute;

        public EvidenceNoteFilterEndDateAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            var httpContextService = A.Fake<IHttpContextService>();

            attribute = new EvidenceNoteFilterEndDateAttribute("StartDate")
            {
                Client = () => client, HttpContextService = httpContextService
            };
        }

        [Fact]
        public void EvidenceNoteFilterEndDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteFilterEndDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteFilterEndDateAttribute_GivenEndDateDateIsBeforeStartDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 2);
            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate.AddDays(-1) };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Ensure the end date is after the start date");
        }

        public static IEnumerable<object[]> ValidEndDatesAfterSpecifiedStartDate =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 2), new DateTime(2020, 1, 2) },
                new object[] { new DateTime(2020, 1, 2), new DateTime(2020, 1, 3) }
            };

        [Theory]
        [MemberData(nameof(ValidEndDatesAfterSpecifiedStartDate))]
        public void EvidenceNoteFilterEndDateAttribute_GivenEndDateDateIsAfterOrEqualToStartDate_ValidationExceptionShouldNotBeThrown(DateTime startDate, DateTime endDate)
        {
            //arrange
            var target = new ValidationTarget() { StartDate = startDate, EndDate = endDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public void EvidenceNoteFilterEndDateAttribute_GivenStartDateIsEmpty_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var target = new ValidationTarget() { StartDate = DateTime.MinValue, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public void EvidenceNoteFilterEndDateAttribute_GivenStartDateIsNull_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var target = new ValidationTarget() { StartDate = null, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        private class ValidationTarget
        {
            public DateTime? StartDate { get; set; }

            [EvidenceNoteFilterEndDate(nameof(StartDate))]
            public DateTime? EndDate { get; set; }
        }
    }
}
