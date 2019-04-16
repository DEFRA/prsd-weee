namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.Domain;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using Xunit;
    using DomainAatf = Domain.AatfReturn.Aatf;
    using DomainScheme = Domain.Scheme.Scheme;
    using Organisation = Domain.Organisation.Organisation;

    public class ReturnMapTests
    {
        private readonly ReturnMap map;
        private readonly DomainAatf aatf;
        private readonly DomainScheme scheme;
        private readonly Organisation organisation;
        private readonly Operator @operator;

        public ReturnMapTests()
        {
            map = new ReturnMap(A.Fake<IMapper>());
            aatf = A.Fake<DomainAatf>();
            scheme = A.Fake<DomainScheme>();
            organisation = Organisation.CreatePartnership("trading name");
            @operator = @operator = new Operator(organisation);
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_QuarterPropertiesShouldBeMapped()
        {
            var source = new ReturnQuarterWindow(GetReturn(), A.Fake<Domain.DataReturns.QuarterWindow>(), 
                null, null, null, null, null, null, A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);

            result.Quarter.Q.Should().Be(EA.Weee.Core.DataReturns.QuarterType.Q1);
            result.Quarter.Year.Should().Be(2019);
        }

        [Fact]
        public void Map_GivenSource_QuarterWindowPropertiesShouldBeMapped()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var quarterWindow = new Domain.DataReturns.QuarterWindow(startTime, endTime);

            var source = new ReturnQuarterWindow(GetReturn(), quarterWindow, null, null, null, null, null, null, A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);

            result.QuarterWindow.EndDate.Should().Be(endTime);
            result.QuarterWindow.StartDate.Should().Be(startTime);
        }

        [Fact]
        public void Map_GivenSource_OperatorShouldBeMapped()
        {
            var @return = GetReturn();
            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), 
                A.Fake<List<DomainAatf>>(), A.Fake<List<NonObligatedWeee>>(), 
                A.Fake<List<WeeeReceivedAmount>>(), A.Fake<List<WeeeReusedAmount>>(), @operator,
                A.Fake<List<WeeeSentOnAmount>>(),
                A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);

            result.ReturnOperatorData.OperatorName.Should().Be(@operator.Organisation.TradingName);
            result.ReturnOperatorData.OrganisationId.Should().Be(@operator.Organisation.Id);
            result.ReturnOperatorData.Id.Should().Be(@operator.Id);
        }

        [Fact]
        public void Map_GivenSource_WeeeSentOnIdShouldBeMapped()
        {
            var @return = GetReturn();
            var obligatedWeeeSentOn = A.Fake<List<WeeeSentOnAmount>>();

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), A.Fake<List<DomainAatf>>(), A.Fake<List<NonObligatedWeee>>(), A.Fake<List<WeeeReceivedAmount>>(), A.Fake<List<WeeeReusedAmount>>(), obligatedWeeeSentOn, @operator);

            var result = map.Map(source);

            for (var i = 0; i < result.ObligatedWeeeSentOnData.Count; i++)
            {
                result.ObligatedWeeeSentOnData[i].Id.Should().Be(obligatedWeeeSentOn[i].Id);
            }
        }

        [Fact]
        public void Map_GivenSource_NonObligatedValuesShouldBeMapped()
        {
            var @return = GetReturn();

            var nonObligated = new List<NonObligatedWeee>()
            {
                new NonObligatedWeee(@return, 1, true, 2),
                new NonObligatedWeee(@return, 2, false, 3)
            };
            
            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), null, nonObligated,  
                null, null, null, null, A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);

            result.NonObligatedData.Count(n => n.CategoryId == 1 && n.Dcf && n.Tonnage == 2).Should().Be(1);
            result.NonObligatedData.Count(n => n.CategoryId == 2 && !n.Dcf && n.Tonnage == 3).Should().Be(1);
            result.NonObligatedData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ObligatedWeeeReceivedValuesShouldBeMapped()
        {
            var @return = GetReturn();
            
            var weeeReceived = ReturnWeeeReceived(scheme, aatf, @return.Id);

            var obligated = new List<WeeeReceivedAmount>()
            {
                new WeeeReceivedAmount(weeeReceived, 1, 1.000m, 2.000m),
                new WeeeReceivedAmount(weeeReceived, 2, 3.000m, 4.000m)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), 
                A.Fake<List<Aatf>>(), A.Fake<List<NonObligatedWeee>>(), obligated, 
                A.Fake<List<WeeeReusedAmount>>(), @operator,
                A.Fake<List<WeeeSentOnAmount>>(),
                A.Fake<List<ReturnScheme>>());
          
            var result = map.Map(source);

            result.ObligatedWeeeReceivedData.Count(o => o.CategoryId == 1 && o.B2C == 1 && o.B2B == 2).Should().Be(1);
            result.ObligatedWeeeReceivedData.Count(o => o.CategoryId == 2 && o.B2C == 3 && o.B2B == 4).Should().Be(1);
            result.ObligatedWeeeReceivedData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ObligatedWeeeReusedValuesShouldBeMapped()
        {
            var @return = GetReturn();

            var weeeReused = ReturnWeeeReused(aatf, @return.Id);

            var obligated = new List<WeeeReusedAmount>()
            {
                new WeeeReusedAmount(weeeReused, 1, 1.000m, 2.000m),
                new WeeeReusedAmount(weeeReused, 2, 3.000m, 4.000m)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), 
                A.Fake<List<Aatf>>(), A.Fake<List<NonObligatedWeee>>(), A.Fake<List<WeeeReceivedAmount>>(), 
                obligated, @operator, A.Fake<List<WeeeSentOnAmount>>(), A.Fake<List<ReturnScheme>>());
          
            var result = map.Map(source);

            result.ObligatedWeeeReusedData.Count(o => o.CategoryId == 1 && o.B2C == 1 && o.B2B == 2).Should().Be(1);
            result.ObligatedWeeeReusedData.Count(o => o.CategoryId == 2 && o.B2C == 3 && o.B2B == 4).Should().Be(1);
            result.ObligatedWeeeReusedData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ObligatedWeeeSentOnValuesShouldBeMapped()
        {
            var @return = GetReturn();
            var siteAddress = new AatfAddress("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", A.Fake<Country>());

            var weeeSentOn = ReturnWeeeSentOn(siteAddress, aatf, @return);

            var obligated = new List<WeeeSentOnAmount>()
            {
                new WeeeSentOnAmount(weeeSentOn, 1, 1.000m, 2.000m, weeeSentOn.Id),
                new WeeeSentOnAmount(weeeSentOn, 2, 3.000m, 4.000m, weeeSentOn.Id)
            };
            
            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), A.Fake<List<Aatf>>(), A.Fake<List<NonObligatedWeee>>(), 
                A.Fake<List<WeeeReceivedAmount>>(), A.Fake<List<WeeeReusedAmount>>(), @operator, obligated, A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);

            result.ObligatedWeeeSentOnData.Count(o => o.CategoryId == 1 && o.B2C == 1 && o.B2B == 2).Should().Be(1);
            result.ObligatedWeeeSentOnData.Count(o => o.CategoryId == 2 && o.B2C == 3 && o.B2B == 4).Should().Be(1);
            result.ObligatedWeeeSentOnData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_AatfsShouldBeMapped()
        {
            var @return = GetReturn();

            var aatfs = new List<Aatf>()
            {
                new Aatf("Aatf1", A.Fake<UKCompetentAuthority>(), "1234", AatfStatus.Approved, @operator),
                new Aatf("Aatf2", A.Fake<UKCompetentAuthority>(), "1234", AatfStatus.Approved, @operator)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), 
                aatfs, A.Fake<List<NonObligatedWeee>>(), A.Fake<List<WeeeReceivedAmount>>(), 
                A.Fake<List<WeeeReusedAmount>>(), @operator, A.Fake<List<WeeeSentOnAmount>>(), A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);

            var zeroGuid = new Guid();

            result.Aatfs.Count(a => a.Name == "Aatf1" && a.Id == zeroGuid).Should().Be(1);
            result.Aatfs.Count(a => a.Name == "Aatf2" && a.Id == zeroGuid).Should().Be(1);
            result.Aatfs.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ReturnSchemesShouldBeMapped()
        {
            var source = new ReturnQuarterWindow(GetReturn(), A.Fake<Domain.DataReturns.QuarterWindow>(),
                null, null, null, null, null, null, A.Fake<List<ReturnScheme>>());

            var result = map.Map(source);
        }

        public Return GetReturn()
        {
            var quarter = new Quarter(2019, QuarterType.Q1);
            var @return = new Return(@operator, quarter, ReturnStatus.Created);

            return @return;
        }

        public WeeeReceived ReturnWeeeReceived(DomainScheme scheme, DomainAatf aatf, Guid returnId)
        {
            var weeeReceived = new WeeeReceived(scheme, aatf, returnId);

            return weeeReceived;
        }

        public WeeeReused ReturnWeeeReused(DomainAatf aatf, Guid returnId)
        {
            var weeeReused = new WeeeReused(aatf, returnId);

            return weeeReused;
        }

        public WeeeSentOn ReturnWeeeSentOn(AatfAddress siteAddress, DomainAatf aatf, Return @return)
        {
            var weeeReused = new WeeeSentOn(siteAddress, aatf, @return);

            return weeeReused;
        }

        public Domain.DataReturns.QuarterWindow GetQuarterWindow()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var quarterWindow = new Domain.DataReturns.QuarterWindow(startTime, endTime);

            return quarterWindow;
        }

        public Domain.DataReturns.QuarterWindow GetQuarterWindowWithOperator()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var quarterWindow = new Domain.DataReturns.QuarterWindow(startTime, endTime);

            return quarterWindow;
        }
    }
}
