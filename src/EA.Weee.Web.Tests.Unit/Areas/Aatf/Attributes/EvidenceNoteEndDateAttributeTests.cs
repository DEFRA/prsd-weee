namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using Api.Client;
    using AutoFixture;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.Areas.Aatf.Attributes;
    using Web.ViewModels.Shared;
    using Weee.Requests.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNoteEndDateAttributeTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient client;
        private readonly IHttpContextService httpContextService;
        private readonly IWeeeCache cache;
        private readonly EvidenceNoteEndDateAttribute attribute;
        private readonly DateTime currentDate;
        private readonly Guid organisationId;
        private readonly Guid aatfId;
        private const string AatfApprovalError = "Aatf approval error";

        public EvidenceNoteEndDateAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            httpContextService = A.Fake<IHttpContextService>();
            cache = A.Fake<IWeeeCache>();

            organisationId = TestFixture.Create<Guid>();
            aatfId = TestFixture.Create<Guid>();

            attribute = new EvidenceNoteEndDateAttribute("StartDate", AatfApprovalError)
            {
                Client = () => client, HttpContextService = httpContextService, Cache = cache
            };

            currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    ApprovalDate = new DateTime(2020, 1, 1),
                    ComplianceYear = (short)currentDate.Year,
                    Id = aatfId,
                    AatfId = aatfId
                }
            });
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteEndDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_CurrentDateShouldBeRetrievedFromApi()
        {
            //arrange
            var target = GetValidationDefaultTarget(currentDate, currentDate);
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

            var target = GetValidationDefaultTarget(currentDate, currentDate.AddDays(-1));
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Ensure the end date is after the start date");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsInJanuaryAfterCurrentComplianceYear_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            var outOfComplianceYear = new DateTime(2021, 1, 1);

            var target = GetValidationDefaultTarget(currentDate, outOfComplianceYear);
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("The end date must be within the current compliance year");
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

            var target = GetValidationDefaultTarget(endDate.AddDays(-1), endDate);
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

            var target = GetValidationDefaultTarget(endDate.AddDays(-1), endDate);
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

            var target = GetValidationDefaultTarget(endDate.AddDays(-1), endDate);
            var context = new ValidationContext(target);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.EndDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("The end date must be within the current compliance year");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenEndDateIsAfterCurrentComplianceYearAndCheckComplianceYearIsTrue_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            var outOfComplianceYear = new DateTime(2021, 1, 1);

            var target = GetValidationDefaultTarget(currentDate, outOfComplianceYear);
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
            var target = GetValidationDefaultTarget(DateTime.MinValue, currentDate);
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

            var target = GetValidationDefaultTarget(null, currentDate);
            var context = new ValidationContext(target);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenModelIsNotBasedOnEvidenceNoteViewModel_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var target = new InvalidValidationTarget() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Unable to validate the evidence note details");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenValidStartAndEndDates_AatfsForOrganisationShouldBeRetrievedFromCache()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(currentDate, currentDate);
            target.OrganisationId = organisationId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            attribute.Validate(target.StartDate, context);

            //assert
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenValidStartAndEndDatesAndNoAatfCouldBeFound_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(currentDate, currentDate);
            target.OrganisationId = organisationId;
            target.AatfId = aatfId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>());

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Aatf is invalid to save evidence notes.");
        }

        [Fact]
        public void EvidenceNoteEndDateAttribute_GivenValidStartAndEndDatesAndNoAatfCouldBeFoundWithApprovalDateBeforeEnteredDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var groupedAatfId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(currentDate, currentDate);
            target.OrganisationId = organisationId;
            target.AatfId = aatfId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)currentDate.Year,
                    ApprovalDate = currentDate.AddDays(1)
                },
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = TestFixture.Create<Guid>(),
                    ComplianceYear = (short)currentDate.Year,
                    ApprovalDate = currentDate.AddDays(1)
                },
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)(currentDate.Year + 1),
                    ApprovalDate = currentDate
                },
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)(currentDate.Year - 1),
                    ApprovalDate = currentDate
                }
            });

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be(AatfApprovalError);
        }

        public static IEnumerable<object[]> ValidEndAndApprovalDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 1, 1) },
                new object[] { new DateTime(2020, 1, 2), new DateTime(2020, 1, 1) },
            };

        [Theory]
        [MemberData(nameof(ValidEndAndApprovalDates))]
        public void EvidenceNoteEndDateAttribute_GivenValidStartAndEndDatesAndAatfCanBeFoundWithApprovalDateBeforeEnteredDate_NoValidationExceptionShouldBeThrown(DateTime endDate, DateTime approvalDate)
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var groupedAatfId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(endDate, endDate);
            target.OrganisationId = organisationId;
            target.AatfId = aatfId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(endDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)endDate.Year,
                    ApprovalDate = approvalDate
                }
            });

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        private ValidationTarget GetValidationDefaultTarget(DateTime? startDate, DateTime endDateTime)
        {
            return new ValidationTarget() { StartDate = startDate, EndDate = endDateTime, AatfId = aatfId, OrganisationId = organisationId };
        }

        private class ValidationTarget : EvidenceNoteViewModel
        {
            public override DateTime? StartDate { get; set; }

            [EvidenceNoteEndDate(nameof(StartDate), AatfApprovalError)]
            public override DateTime? EndDate { get; set; }
        }

        private class InvalidValidationTarget
        {
           public DateTime? StartDate { get; set; }

            [EvidenceNoteStartDate(nameof(StartDate), AatfApprovalError)]
            public DateTime? EndDate { get; set; }
        }
    }
}
