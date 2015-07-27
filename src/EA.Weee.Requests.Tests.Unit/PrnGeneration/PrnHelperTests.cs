namespace EA.Weee.Requests.Tests.Unit.PrnGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Core.Helpers.PrnGeneration;
    using Xunit;

    public class PrnGenerationTests
    {
        [Fact]
        public void PrnFromSeed_PassedZero_RecomputedSeedValueEqualsZero()
        {
            const uint Value = 0;

            PrnSeedHelper prnSeedHelper = new PrnSeedHelper(Value);

            uint seedFromPrn = prnSeedHelper.ToSeedValue();

            Assert.Equal(Value, seedFromPrn);
        }

        [Fact]
        public void PrnFromSeed_PassedValueOutOfRange_RecomputedSeedValueIsInRange()
        {
            const uint Value = 9967;
            const uint NextValueInRange = 65536;

            PrnSeedHelper prnSeedHelper = new PrnSeedHelper(Value);

            uint seedFromPrn = prnSeedHelper.ToSeedValue();

            Assert.Equal(NextValueInRange, seedFromPrn);
        }

        [Fact]
        public void PrnAsIntegerToString_PassedSummat_ReturnsSummat()
        {
            Debug.WriteLine("Start! {0}", DateTime.Now.TimeOfDay);

            const uint InitialSeed = 0;
            uint currentSeed = InitialSeed;

            var usedPrns = new HashSet<string>();

            for (uint counter = 0; currentSeed < uint.MaxValue / 10; counter++)
            {
                var prn = new PrnHelper(new QuadraticResidueHelper()).ComputePrnFromSeed(ref currentSeed);

                Assert.StartsWith("WEE/", prn);

                Assert.InRange(prn[4], 'A', 'N');
                Assert.InRange(prn[5], 'A', 'N');
                Assert.InRange(prn[6], '0', '9');
                Assert.InRange(prn[7], '0', '9');
                Assert.InRange(prn[8], '0', '9');
                Assert.InRange(prn[9], '0', '9');
                Assert.InRange(prn[10], 'M', 'Z');
                Assert.InRange(prn[11], 'M', 'Z');

                if (usedPrns.Contains(prn))
                {
                    Assert.False(usedPrns.Contains(prn), string.Format("prn {0} already present in usedPrns!", prn));
                }
                
                usedPrns.Add(prn);

                //Debug.WriteLine(string.Format("{0}/{1}: {2}", currentSeed, uint.MaxValue, result.Item1));
            }

            Debug.WriteLine("End! Last few PRNs were {0}. {1}", string.Join("\n", usedPrns.Skip(Math.Max(0, usedPrns.Count() - 20000))), DateTime.Now.TimeOfDay);
        }
    }
}
