namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.User;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Domain;
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
        private readonly IMapper mapper;
        private readonly IMap<Organisation, OrganisationData> organisationMapper;
        private readonly IMap<AatfStatus, Core.AatfReturn.AatfStatus> statusMapper;
        private readonly Organisation organisation;

        public ReturnMapTests()
        {
            mapper = A.Fake<IMapper>();
            organisationMapper = A.Fake<IMap<Organisation, OrganisationData>>();
            statusMapper = A.Fake<IMap<AatfStatus, Core.AatfReturn.AatfStatus>>();

            map = new ReturnMap(mapper, organisationMapper, statusMapper);

            aatf = A.Fake<DomainAatf>();
            scheme = A.Fake<DomainScheme>();
            organisation = Organisation.CreatePartnership("trading name");
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
                null, null, null, null, null, null, A.Fake<List<ReturnScheme>>(), A.Fake<List<ReturnReportOn>>(), A.Dummy<DateTime>());

            var result = map.Map(source);

            result.Quarter.Q.Should().Be(EA.Weee.Core.DataReturns.QuarterType.Q1);
            result.Quarter.Year.Should().Be(2019);
        }

        [Fact]
        public void Map_GivenSource_QuarterWindowPropertiesShouldBeMapped()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddDays(1);
            var quarterWindow = new Domain.DataReturns.QuarterWindow(startTime, endTime, QuarterType.Q1);

            var source = new ReturnQuarterWindow(GetReturn(), quarterWindow,
                null, null, null, null, null, null, A.Fake<List<ReturnScheme>>(), A.Fake<List<ReturnReportOn>>(), A.Dummy<DateTime>());

            var result = map.Map(source);

            result.QuarterWindow.QuarterEnd.Should().Be(endTime);
            result.QuarterWindow.WindowOpenDate.Should().Be(startTime);
        }

        [Fact]
        public void Map_GivenSource_OrganisationShouldBeMapped()
        {
            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(),
                A.Fake<List<DomainAatf>>(), A.Fake<List<NonObligatedWeee>>(),
                A.Fake<List<WeeeReceivedAmount>>(), A.Fake<List<WeeeReusedAmount>>(), organisation,
                A.Fake<List<WeeeSentOnAmount>>(),
                A.Fake<List<ReturnScheme>>(),
                A.Fake<List<ReturnReportOn>>(),
                A.Dummy<DateTime>());

            var organisationData = new OrganisationData() { Name = "name", Id = Guid.NewGuid() };

            A.CallTo(() => organisationMapper.Map(source.Organisation)).Returns(organisationData);

            var result = map.Map(source);

            result.OrganisationData.Should().Be(organisationData);
        }

        [Fact]
        public void Map_GivenSource_WeeeSentOnIdShouldBeMapped()
        {
            var obligatedWeeeSentOn = A.Fake<List<WeeeSentOnAmount>>();

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(),
                A.Fake<List<DomainAatf>>(), A.Fake<List<NonObligatedWeee>>(), A.Fake<List<WeeeReceivedAmount>>(), A.Fake<List<WeeeReusedAmount>>(),
                organisation, obligatedWeeeSentOn, A.Fake<List<ReturnScheme>>(), A.Fake<List<ReturnReportOn>>(), A.Dummy<DateTime>());

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
                null, null, null, null, A.Fake<List<ReturnScheme>>(), A.Fake<List<ReturnReportOn>>(), A.Dummy<DateTime>());

            var result = map.Map(source);

            result.NonObligatedData.Count(n => n.CategoryId == 1 && n.Dcf && n.Tonnage == 2).Should().Be(1);
            result.NonObligatedData.Count(n => n.CategoryId == 2 && !n.Dcf && n.Tonnage == 3).Should().Be(1);
            result.NonObligatedData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ObligatedWeeeReceivedValuesShouldBeMapped()
        {
            var @return = GetReturn();

            var weeeReceived = ReturnWeeeReceived(scheme, aatf, @return);

            var obligated = new List<WeeeReceivedAmount>()
            {
                new WeeeReceivedAmount(weeeReceived, 1, 1.000m, 2.000m),
                new WeeeReceivedAmount(weeeReceived, 2, 3.000m, 4.000m)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(),
                A.Fake<List<Aatf>>(), A.Fake<List<NonObligatedWeee>>(), obligated,
                A.Fake<List<WeeeReusedAmount>>(), organisation,
                A.Fake<List<WeeeSentOnAmount>>(),
                A.Fake<List<ReturnScheme>>(),
                A.Fake<List<ReturnReportOn>>(),
                A.Dummy<DateTime>());

            var result = map.Map(source);

            result.ObligatedWeeeReceivedData.Count(o => o.CategoryId == 1 && o.B2C == 1 && o.B2B == 2).Should().Be(1);
            result.ObligatedWeeeReceivedData.Count(o => o.CategoryId == 2 && o.B2C == 3 && o.B2B == 4).Should().Be(1);
            result.ObligatedWeeeReceivedData.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ObligatedWeeeReusedValuesShouldBeMapped()
        {
            var @return = GetReturn();

            var weeeReused = ReturnWeeeReused(aatf, @return);

            var obligated = new List<WeeeReusedAmount>()
            {
                new WeeeReusedAmount(weeeReused, 1, 1.000m, 2.000m),
                new WeeeReusedAmount(weeeReused, 2, 3.000m, 4.000m)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(),
                A.Fake<List<Aatf>>(), A.Fake<List<NonObligatedWeee>>(), A.Fake<List<WeeeReceivedAmount>>(),
                obligated, organisation, A.Fake<List<WeeeSentOnAmount>>(), A.Fake<List<ReturnScheme>>(),
                A.Fake<List<ReturnReportOn>>(),
                A.Dummy<DateTime>());

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
                new WeeeSentOnAmount(weeeSentOn, 1, 1.000m, 2.000m),
                new WeeeSentOnAmount(weeeSentOn, 2, 3.000m, 4.000m)
            };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(), A.Fake<List<Aatf>>(), A.Fake<List<NonObligatedWeee>>(),
                A.Fake<List<WeeeReceivedAmount>>(), A.Fake<List<WeeeReusedAmount>>(), organisation, obligated, A.Fake<List<ReturnScheme>>(),
                A.Fake<List<ReturnReportOn>>(),
                A.Dummy<DateTime>());

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
                new Aatf("Aatf1", A.Fake<UKCompetentAuthority>(), "1234", AatfStatus.Approved, organisation, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>()),
                new Aatf("Aatf2", A.Fake<UKCompetentAuthority>(), "1234", AatfStatus.Approved, organisation, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>()),
            };

            foreach (Aatf aatf in aatfs)
            {
                A.CallTo(() => statusMapper.Map(aatf.AatfStatus)).Returns(Core.AatfReturn.AatfStatus.Approved);
            }

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(),
                aatfs, A.Fake<List<NonObligatedWeee>>(), A.Fake<List<WeeeReceivedAmount>>(),
                A.Fake<List<WeeeReusedAmount>>(), organisation, A.Fake<List<WeeeSentOnAmount>>(), A.Fake<List<ReturnScheme>>(),
                A.Fake<List<ReturnReportOn>>(),
                A.Dummy<DateTime>());

            var result = map.Map(source);

            var zeroGuid = new Guid();

            result.Aatfs.Count(a => a.Name == "Aatf1" && a.Id == zeroGuid && a.AatfStatus.Value == AatfStatus.Approved.Value).Should().Be(1);
            result.Aatfs.Count(a => a.Name == "Aatf2" && a.Id == zeroGuid && a.AatfStatus.Value == AatfStatus.Approved.Value).Should().Be(1);
            result.Aatfs.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenSource_ReturnReportOnsShouldBeMapped()
        {
            var @return = GetReturn();
            var returnReportOnList = new List<ReturnReportOn> { new ReturnReportOn(@return.Id, 1), new ReturnReportOn(@return.Id, 3) };

            var source = new ReturnQuarterWindow(GetReturn(), GetQuarterWindow(),
                null, null, null, null, null, null, A.Fake<List<ReturnScheme>>(), returnReportOnList, A.Dummy<DateTime>());

            var result = map.Map(source);

            result.ReturnReportOns.Count.Should().Be(2);
            result.ReturnReportOns.Count(r => r.ReportOnQuestionId == 1).Should().Be(1);
            result.ReturnReportOns.Count(r => r.ReportOnQuestionId == 3).Should().Be(1);
        }

        [Fact]
        public void Map_GivenSourceCreatedBy_CreatedByValuesShouldBeMapped()
        {
            var @returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            var @return = A.Fake<Return>();

            A.CallTo(() => @returnQuarterWindow.QuarterWindow).Returns(GetQuarterWindow());
            A.CallTo(() => @returnQuarterWindow.Return).Returns(@return);
            A.CallTo(() => @return.Quarter).Returns(new Quarter(2019, QuarterType.Q1));
            A.CallTo(() => @return.CreatedBy).Returns(new User("id", "first", "surname", "email"));
            A.CallTo(() => @return.CreatedDate).Returns(new DateTime(2019, 01, 01));

            var result = map.Map(@returnQuarterWindow);

            result.CreatedBy.Should().Be("first surname");
            result.CreatedDate.Should().Be(new DateTime(2019, 01, 01));
            result.SubmittedDate.Should().BeNull();
            result.SubmittedBy.Should().Be(" ");
        }

        [Fact]
        public void Map_GivenSourceSubmitted_SubmittedByValuesShouldBeMapped()
        {
            var @returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            var @return = A.Fake<Return>();

            A.CallTo(() => @returnQuarterWindow.QuarterWindow).Returns(GetQuarterWindow());
            A.CallTo(() => @returnQuarterWindow.Return).Returns(@return);
            A.CallTo(() => @return.Quarter).Returns(new Quarter(2019, QuarterType.Q1));
            A.CallTo(() => @return.SubmittedBy).Returns(new User("id", "first", "surname", "email"));
            A.CallTo(() => @return.SubmittedDate).Returns(new DateTime(2019, 01, 01));

            var result = map.Map(@returnQuarterWindow);

            result.SubmittedBy.Should().Be("first surname");
            result.SubmittedDate.Should().Be(new DateTime(2019, 01, 01));
        }

        [Fact]
        public void Map_GivenSourceStatus_ReturnStatusShouldBeMapped()
        {
            var @returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            var @return = GetReturn();
            var returnStatusMap = new ReturnStatusMap();

            @return.ReturnStatus = ReturnStatus.Submitted;

            A.CallTo(() => @returnQuarterWindow.QuarterWindow).Returns(GetQuarterWindow());
            A.CallTo(() => @returnQuarterWindow.Return).Returns(@return);
            A.CallTo(() => mapper.Map<Core.AatfReturn.ReturnStatus>(@return.ReturnStatus)).Returns(Core.AatfReturn.ReturnStatus.Created);

            var result = map.Map(@returnQuarterWindow);
            result.ReturnStatus.Should().Be(Core.AatfReturn.ReturnStatus.Created);
        }

        [Fact]
        public void Map_GivenReturnSchemes_ReturnSchemeDataItemsShouldBeMapped()
        {
            var @returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            var @return = GetReturn();

            var returnSchemes = new List<ReturnScheme>()
            {
                new ReturnScheme(A.Fake<DomainScheme>(), @return),
                new ReturnScheme(A.Fake<DomainScheme>(), @return)
            };

            var schemeData = new List<SchemeData>()
            {
                new SchemeData(),
                new SchemeData()
            };

            A.CallTo(() => @returnQuarterWindow.QuarterWindow).Returns(GetQuarterWindow());
            A.CallTo(() => @returnQuarterWindow.Return).Returns(@return);
            A.CallTo(() => @returnQuarterWindow.ReturnSchemes).Returns(returnSchemes);
            A.CallTo(() => mapper.Map<EA.Weee.Domain.Scheme.Scheme, SchemeData>(returnSchemes.ElementAt(0).Scheme)).Returns(schemeData.ElementAt(0));
            A.CallTo(() => mapper.Map<EA.Weee.Domain.Scheme.Scheme, SchemeData>(returnSchemes.ElementAt(1).Scheme)).Returns(schemeData.ElementAt(1));

            var result = map.Map(@returnQuarterWindow);

            result.SchemeDataItems.Should().Contain(schemeData.ElementAt(0));
            result.SchemeDataItems.Should().Contain(schemeData.ElementAt(1));
            result.SchemeDataItems.Count().Should().Be(2);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_GivenSourceNilReturn_NilReturnShouldBeMapped(bool nilReturn)
        {
            var @returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            var @return = A.Fake<Return>();

            A.CallTo(() => @returnQuarterWindow.QuarterWindow).Returns(GetQuarterWindow());
            A.CallTo(() => @returnQuarterWindow.Return).Returns(@return);
            A.CallTo(() => @return.Quarter).Returns(new Quarter(2019, QuarterType.Q1));
            A.CallTo(() => @return.NilReturn).Returns(nilReturn);

            var result = map.Map(@returnQuarterWindow);

            result.NilReturn.Should().Be(nilReturn);
        }

        public Return GetReturn()
        {
            var quarter = new Quarter(2019, QuarterType.Q1);
            var @return = new Return(organisation, quarter, Guid.NewGuid().ToString(), FacilityType.Aatf) { CreatedBy = new User("id", "first", "surname", "email") };

            return @return;
        }

        [Fact]
        public void Map_GivenSystemDateTime_SystemDateTimePropertyShouldBeMapped()
        {
            var date = new DateTime(2019, 1, 1);

            var @returnQuarterWindow = A.Fake<ReturnQuarterWindow>();
            
            A.CallTo(() => @returnQuarterWindow.SystemDateTime).Returns(date);

            var result = map.Map(@returnQuarterWindow);

            result.SystemDateTime.Should().Be(date);
        }

        public WeeeReceived ReturnWeeeReceived(DomainScheme scheme, DomainAatf aatf, Return @return)
        {
            var weeeReceived = new WeeeReceived(scheme, aatf, @return);

            return weeeReceived;
        }

        public WeeeReused ReturnWeeeReused(DomainAatf aatf, Return @return)
        {
            var weeeReused = new WeeeReused(aatf, @return);

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
            var quarterWindow = new Domain.DataReturns.QuarterWindow(startTime, endTime, QuarterType.Q1);

            return quarterWindow;
        }

        public ReturnQuarterWindow GetReturnQuarterWindow()
        {
            return new ReturnQuarterWindow(GetReturn(), A.Fake<Domain.DataReturns.QuarterWindow>(),
                 null, null, null, null, null, null, A.Fake<List<ReturnScheme>>(), null, A.Dummy<DateTime>());
        }
    }
}
