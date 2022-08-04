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
    using Weee.Requests.Shared;
    using Xunit;

    public class EvidenceNoteEndDateAttributeTests
    {
        private readonly IWeeeClient client;
        private readonly IHttpContextService httpContextService;
        private readonly EvidenceNoteEndDateAttribute attribute;
        private readonly EvidenceNoteEndDateAttribute attributeWithNoComplianceYearCheck;
        private readonly DateTime currentDate;

        public EvidenceNoteEndDateAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            httpContextService = A.Fake<IHttpContextService>();

            attribute = new EvidenceNoteEndDateAttribute("StartDate", true)
            {
                Client = () => client, HttpContextService = httpContextService
            };

            attributeWithNoComplianceYearCheck = new EvidenceNoteEndDateAttribute("StartDate", false)
            {
                Client = () => client, HttpContextService = httpContextService
            };

            currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
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
            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            var userToken = "token";
            A.CallTo(() => httpContextService.GetAccessToken()).Returns(userToken);

            //act
            attribute.Validate(target.EndDate, context);

            //assert
            A.CallTo(() => client.SendAsync(userToken, A<GetApiDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateDateIsBeforeStartDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 2);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate, EndDate = currentDate.AddDays(-1) };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Ensure the end date is after the start date");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsInJanuaryAfterCurrentComplianceYear_ValidationExceptionShouldNotBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            var outOfComplianceYear = new DateTime(2021, 1, 1);

            var target = new ValidationTargetWithoutComplianceYearCheck() { StartDate = currentDate, EndDate = outOfComplianceYear };
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attributeWithNoComplianceYearCheck.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> ValidNextYearDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2021, 1, 31) },
                new object[] { new DateTime(2021, 1, 1) }
            };

        [Theory]
        [MemberData(nameof(ValidNextYearDates))]
        public void EvidenceNoteEndDateAttribute_GivenCurrentDateIsWithinTheFirstMonthOfNextComplianceShouldAllowPreviousComplianceYear_ValidationExceptionShouldNotBeThrown(DateTime currentDate)
        {
            //arrange
            var endDate = new DateTime(2020, 12, 31);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = endDate.AddDays(-1), EndDate = endDate };
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> InValidEndDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2018, 12, 31) },
                new object[] { new DateTime(2021, 12, 31) }
            };

        [Theory]
        [MemberData(nameof(InValidEndDates))]

        public void EvidenceNoteEndDateAttribute_GivenCurrentDateIsWithinTheFirstMonthOfNextComplianceButEndDateIsOutsideOfPreviousYear_ValidationExceptionShouldBeThrown(DateTime endDate)
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = endDate.AddDays(-1), EndDate = endDate };
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("The end date must be within the current compliance year");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenCurrentDateIsOutsideTheFirstMonthOfNextComplianceShouldNotAllowPreviousComplianceYear_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var endDate = new DateTime(2020, 12, 31);
            var currentDate = new DateTime(2021, 2, 1);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = endDate.AddDays(-1), EndDate = endDate };
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("The end date must be within the current compliance year");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsAfterCurrentComplianceYearAndCheckComplianceYearIsFalse_ValidationExceptionShouldNotBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            var outOfComplianceYear = new DateTime(2021, 1, 1);

            var target = new ValidationTargetWithoutComplianceYearCheck() { StartDate = currentDate, EndDate = outOfComplianceYear };
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attributeWithNoComplianceYearCheck.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsAfterCurrentComplianceYearAndCheckComplianceYearIsTrue_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            var outOfComplianceYear = new DateTime(2021, 1, 1);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = currentDate, EndDate = outOfComplianceYear };
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("The end date must be within the current compliance year");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenStartDateIsEmpty_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = DateTime.MinValue, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenStartDateIsNull_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);

            var target = new ValidationTargetWithComplianceYearCheck() { StartDate = null, EndDate = currentDate };
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        private class ValidationTargetWithComplianceYearCheck
        {
            public DateTime? StartDate { get; set; }

            [EvidenceNoteEndDate(nameof(StartDate), true)]
            public DateTime? EndDate { get; set; }
        }

        private class ValidationTargetWithoutComplianceYearCheck
        {
            public DateTime? StartDate { get; set; }

            [EvidenceNoteEndDate(nameof(StartDate), false)]
            public DateTime? EndDate { get; set; }
        }
    }
}
