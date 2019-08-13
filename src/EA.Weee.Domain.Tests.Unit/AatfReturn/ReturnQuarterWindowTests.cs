namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnQuarterWindowTests
    {
        public ReturnQuarterWindowTests()
        {
        }

        [Fact]
        public void ReturnQuarterWindow_GivenNullReturn_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var returnQuarterWindow = new ReturnQuarterWindow(null, 
                    A.Dummy<QuarterWindow>(), 
                    new List<Aatf>(), 
                    new List<NonObligatedWeee>(), 
                    new List<WeeeReceivedAmount>(),
                    new List<WeeeReusedAmount>(),
                    A.Dummy<Organisation>(),
                    new List<WeeeSentOnAmount>(),
                    new List<ReturnScheme>(),
                    new List<ReturnReportOn>(),
                    DateTime.Now);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnQuarterWindow_GivenNullQuarterWindow_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var returnQuarterWindow = new ReturnQuarterWindow(A.Dummy<Return>(),
                    null,
                    new List<Aatf>(),
                    new List<NonObligatedWeee>(),
                    new List<WeeeReceivedAmount>(),
                    new List<WeeeReusedAmount>(),
                    A.Dummy<Organisation>(),
                    new List<WeeeSentOnAmount>(),
                    new List<ReturnScheme>(),
                    new List<ReturnReportOn>(),
                    DateTime.Now);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnQuarterWindow_GivenNullReturnScheme_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var returnQuarterWindow = new ReturnQuarterWindow(A.Dummy<Return>(),
                    A.Dummy<QuarterWindow>(),
                    new List<Aatf>(),
                    new List<NonObligatedWeee>(),
                    new List<WeeeReceivedAmount>(),
                    new List<WeeeReusedAmount>(),
                    A.Dummy<Organisation>(),
                    new List<WeeeSentOnAmount>(),
                    null,
                    new List<ReturnReportOn>(),
                    DateTime.Now);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnQuarterWindow_GivenSourceData_PropertiesShouldBeMapped()
        {
            var @return = A.Dummy<Return>();
            var quarterWindow = A.Dummy<QuarterWindow>();
            var aatfs = new List<Aatf>();
            var nonObligatedWeee = new List<NonObligatedWeee>();
            var weeeReceived = new List<WeeeReceivedAmount>();
            var weeeReused = new List<WeeeReusedAmount>();
            var organisation = A.Dummy<Organisation>();
            var weeeSentOn = new List<WeeeSentOnAmount>();
            var returnSchemes = new List<ReturnScheme>();
            var returnReportOn = new List<ReturnReportOn>();
            var date = new DateTime(2019, 1, 1);

            var returnQuarterWindow = new ReturnQuarterWindow(@return,
                quarterWindow,
                aatfs,
                nonObligatedWeee,
                weeeReceived,
                weeeReused,
                organisation,
                weeeSentOn,
                returnSchemes,
                returnReportOn,
                date);

            returnQuarterWindow.Return.Should().Be(@return);
            returnQuarterWindow.QuarterWindow.Should().Be(quarterWindow);
            returnQuarterWindow.Aatfs.Should().BeSameAs(aatfs);
            returnQuarterWindow.NonObligatedWeeeList.Should().BeSameAs(nonObligatedWeee);
            returnQuarterWindow.ObligatedWeeeReceivedList.Should().BeSameAs(weeeReceived);
            returnQuarterWindow.ObligatedWeeeReusedList.Should().BeSameAs(weeeReused);
            returnQuarterWindow.Organisation.Should().Be(organisation);
            returnQuarterWindow.ObligatedWeeeSentOnList.Should().BeSameAs(weeeSentOn);
            returnQuarterWindow.ReturnSchemes.Should().BeSameAs(returnSchemes);
            returnQuarterWindow.ReturnReportOns.Should().BeSameAs(returnReportOn);
            returnQuarterWindow.SystemDateTime.Should().Be(date);
        }
    }
}