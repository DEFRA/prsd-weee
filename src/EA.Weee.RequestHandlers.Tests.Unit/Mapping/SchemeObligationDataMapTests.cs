namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Domain.Scheme;
    using FluentAssertions;
    using Mappings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Domain.Lookup;
    using Domain.Obligation;
    using Weee.Tests.Core;
    using Xunit;

    public class SchemeObligationDataMapTests : SimpleUnitTestBase
    {
        private readonly SchemeObligationDataMap map;

        public SchemeObligationDataMapTests()
        {
            map = new SchemeObligationDataMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var result = Record.Exception(() => map.Map(null));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSchemeSourceWithObligationSchemes_SchemeObligationDataShouldBeMapped()
        {
            //arrange
            var scheme1Name = TestFixture.Create<string>();
            var scheme1 = new Scheme(TestFixture.Create<Guid>());
            ObjectInstantiator<Scheme>.SetProperty(o => o.SchemeName, scheme1Name, scheme1);

            var date = DateTime.Now;
            var scheme1ObligationScheme = new ObligationScheme(scheme1, 2022);
            var scheme1ObligationAmounts1 = new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 1);
            var scheme1ObligationAmounts2 = new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, null);
            var scheme1ObligationAmounts = new List<ObligationSchemeAmount>() { scheme1ObligationAmounts1, scheme1ObligationAmounts2 };

            ObjectInstantiator<ObligationScheme>.SetProperty(o => o.ObligationSchemeAmounts, scheme1ObligationAmounts, scheme1ObligationScheme);
            ObjectInstantiator<ObligationScheme>.SetProperty(o => o.UpdatedDate, date, scheme1ObligationScheme);
            ObjectInstantiator<Scheme>.SetProperty(o => o.ObligationSchemes, new List<ObligationScheme>() { scheme1ObligationScheme }, scheme1);

            var scheme2Name = TestFixture.Create<string>();
            var scheme2 = new Scheme(TestFixture.Create<Guid>());
            ObjectInstantiator<Scheme>.SetProperty(o => o.SchemeName, scheme2Name, scheme2);

            var scheme2ObligationScheme = new ObligationScheme(scheme2, 2022);
            var scheme2ObligationAmounts1 = new ObligationSchemeAmount(WeeeCategory.LightingEquipment, null);
            var scheme2ObligationAmounts2 = new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 200);
            var scheme2ObligationAmounts3 = new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 100);
            var scheme2ObligationAmounts = new List<ObligationSchemeAmount>() { scheme2ObligationAmounts1, scheme2ObligationAmounts2, scheme2ObligationAmounts3 };

            ObjectInstantiator<ObligationScheme>.SetProperty(o => o.ObligationSchemeAmounts, scheme2ObligationAmounts, scheme2ObligationScheme);
            ObjectInstantiator<ObligationScheme>.SetProperty(o => o.Scheme, scheme2, scheme2ObligationScheme);
            ObjectInstantiator<ObligationScheme>.SetProperty(o => o.UpdatedDate, null, scheme2ObligationScheme);
            ObjectInstantiator<Scheme>.SetProperty(o => o.ObligationSchemes, new List<ObligationScheme>() { scheme2ObligationScheme }, scheme2);

            //act
            var result = map.Map(new List<Scheme>() { scheme1, scheme2 });

            //assert
            result.Count.Should().Be(2);
            var scheme1Obligations = result.Where(s => s.SchemeName.Equals(scheme1.SchemeName));
            scheme1Obligations.Count().Should().Be(1);
            scheme1Obligations.ElementAt(0).UpdatedDate.Should().BeSameDateAs(date);
            scheme1Obligations.ElementAt(0).SchemeName.Should().Be(scheme1Name);
            scheme1Obligations.ElementAt(0).SchemeObligationAmountData.Count.Should().Be(2);
            scheme1Obligations.ElementAt(0).SchemeObligationAmountData.Should().Contain(a => a.Obligation == 1 &&
                a.Category.ToInt() == Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment.ToInt());
            scheme1Obligations.ElementAt(0).SchemeObligationAmountData.Should().Contain(a => a.Obligation == null &&
                a.Category.ToInt() == Core.DataReturns.WeeeCategory.PhotovoltaicPanels.ToInt());

            var scheme2Obligations = result.Where(s => s.SchemeName.Equals(scheme2.SchemeName));
            scheme2Obligations.Count().Should().Be(1);
            scheme2Obligations.ElementAt(0).UpdatedDate.Should().BeNull();
            scheme2Obligations.ElementAt(0).SchemeName.Should().Be(scheme2Name);
            scheme2Obligations.ElementAt(0).SchemeObligationAmountData.Count.Should().Be(3);
            scheme2Obligations.ElementAt(0).SchemeObligationAmountData.Should().Contain(a => a.Obligation == null &&
                a.Category.ToInt() == Core.DataReturns.WeeeCategory.LightingEquipment.ToInt());
            scheme2Obligations.ElementAt(0).SchemeObligationAmountData.Should().Contain(a => a.Obligation == 200 &&
                a.Category.ToInt() == Core.DataReturns.WeeeCategory.AutomaticDispensers.ToInt());
            scheme2Obligations.ElementAt(0).SchemeObligationAmountData.Should().Contain(a => a.Obligation == 100 &&
                a.Category.ToInt() == Core.DataReturns.WeeeCategory.LargeHouseholdAppliances.ToInt());
        }

        [Fact]
        public void Map_GivenSchemeSourceWithNoObligationSchemes_SchemeObligationDataShouldBeMapped()
        {
            //arrange
            var scheme1Name = TestFixture.Create<string>();
            var scheme1 = new Scheme(TestFixture.Create<Guid>());
            ObjectInstantiator<Scheme>.SetProperty(o => o.SchemeName, scheme1Name, scheme1);

            var scheme2Name = TestFixture.Create<string>();
            var scheme2 = new Scheme(TestFixture.Create<Guid>());
            ObjectInstantiator<Scheme>.SetProperty(o => o.SchemeName, scheme2Name, scheme2);

            //act
            var result = map.Map(new List<Scheme>() { scheme1, scheme2 });

            //assert
            result.Count.Should().Be(2);
            var scheme1Obligations = result.Where(s => s.SchemeName.Equals(scheme1.SchemeName));
            scheme1Obligations.Count().Should().Be(1);
            scheme1Obligations.ElementAt(0).UpdatedDate.Should().BeNull();
            scheme1Obligations.ElementAt(0).SchemeName.Should().Be(scheme1Name);
            scheme1Obligations.ElementAt(0).SchemeObligationAmountData.Should().BeEmpty();

            var scheme2Obligations = result.Where(s => s.SchemeName.Equals(scheme2.SchemeName));
            scheme2Obligations.Count().Should().Be(1);
            scheme2Obligations.ElementAt(0).UpdatedDate.Should().BeNull();
            scheme2Obligations.ElementAt(0).SchemeName.Should().Be(scheme2Name);
            scheme2Obligations.ElementAt(0).SchemeObligationAmountData.Should().BeEmpty();
        }
    }
}
