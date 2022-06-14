namespace EA.Weee.Domain.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Lookup;
    using Obligation;
    using Prsd.Core;
    using Xunit;

    public class ObligationSchemeTests
    {
        [Fact]
        public void UpdateObligationSchemeAmounts_GivenAmountsToBeUpdated_AmountsShouldBeUpdated()
        {
            //arrange
            var newAmounts = new List<ObligationSchemeAmount>()
            {
                new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 1),
                new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, 2),
                new ObligationSchemeAmount(WeeeCategory.MedicalDevices, null),
            };

            var obligationScheme = new ObligationScheme(A.Fake<Scheme>(), 2022);
            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, null));
            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, 1));
            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.MedicalDevices, 10));

            //act
            obligationScheme.UpdateObligationSchemeAmounts(newAmounts);

            //assert
            obligationScheme.ObligationSchemeAmounts.First(o => o.CategoryId == WeeeCategory.ConsumerEquipment)
                .Obligation.Should().Be(1);
            obligationScheme.ObligationSchemeAmounts.First(o => o.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources)
                .Obligation.Should().Be(2);
            obligationScheme.ObligationSchemeAmounts.First(o => o.CategoryId == WeeeCategory.MedicalDevices)
                .Obligation.Should().BeNull();
        }

        [Fact]
        public void UpdateObligationSchemeAmounts_GivenAmountsAreUpdated_UpdatedDateShouldBeUpdated()
        {
            //arrange
            var date = new DateTime(2020, 6, 10, 10, 1, 2);
            SystemTime.Freeze(date);

            var newAmounts = new List<ObligationSchemeAmount>()
            {
                new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 1),
                new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 1)
            };

            var obligationScheme = new ObligationScheme(A.Fake<Scheme>(), 2022);
            SystemTime.Unfreeze();

            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 2));
            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 1));

            //act
            obligationScheme.UpdateObligationSchemeAmounts(newAmounts);

            //assert
            obligationScheme.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void UpdateObligationSchemeAmounts_GivenAmountsAreNotUpdated_UpdatedDateShouldBeNotBeUpdated()
        {
            //arrange
            var date = new DateTime(2020, 6, 10, 10, 1, 2);
            SystemTime.Freeze(date);

            var newAmounts = new List<ObligationSchemeAmount>()
            {
                new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 2),
                new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 2)
            };

            var obligationScheme = new ObligationScheme(A.Fake<Scheme>(), 2022);
            SystemTime.Unfreeze();

            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 2));
            obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 2));

            //act
            obligationScheme.UpdateObligationSchemeAmounts(newAmounts);

            //assert
            obligationScheme.UpdatedDate.Should().Be(date);
        }
    }
}
