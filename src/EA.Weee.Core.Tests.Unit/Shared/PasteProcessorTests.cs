namespace EA.Weee.Core.Tests.Unit.Shared
{
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Shared;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfEvidence;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class PasteProcessorTests
    {
        private readonly Fixture fixture;
        private readonly PasteProcessor pasteProcessor;

        public PasteProcessorTests()
        {
            fixture = new Fixture();
            pasteProcessor = new PasteProcessor();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BuildModel_GivenNullString_EmptyCategoriesModelExpected(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            result.Should().BeOfType<PastedValues>();
            AssertEmptyValues(result);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1\r\n")]
        [InlineData("1\r")]
        [InlineData("1\n")]
        public void BuildModel_GivenStringContainsSingleNumericValue_CategoryValuesShouldBeOne(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            result.ElementAt(0).Tonnage.Should().Be("1");
        }

        [Theory]
        [InlineData("1\r\n2\r\n")]
        [InlineData("1\r2\r")]
        [InlineData("1\n2\n")]
        public void BuildModel_GivenStringContainsOneColumnsAndTwoRows_CategoryValuesShouldBePopulated(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            result.ElementAt(0).Tonnage.Should().Be("1");
            result.ElementAt(1).Tonnage.Should().Be("2");
        }

        [Theory]
        [InlineData("1,000\r\n2,000\r\n")]
        [InlineData("1,000\r2,000\r")]
        [InlineData("1,000\n2,000\n")]
        public void BuildModel_GivenStringContainsCommaThousandsSeparators_CategoryValuesShouldBePopulation(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            result.ElementAt(0).Tonnage.Should().Be("1,000");
            result.ElementAt(1).Tonnage.Should().Be("2,000");
        }

        [Theory]
        [InlineData("1,000.000\r\n2,000.000\r\n")]
        [InlineData("1,000.000\r2,000.000\r")]
        [InlineData("1,000.000\n2,000.000\n")]
        public void BuildModel_GivenStringContainsCommaThousandsSeparatorsAndDecimals_CategoryValuesShouldBePopulation(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            result.ElementAt(0).Tonnage.Should().Be("1,000.000");
            result.ElementAt(1).Tonnage.Should().Be("2,000.000");
        }

        [Theory]
        [InlineData("1.000\r\n2.000\r\n")]
        [InlineData("1.000\r2.000\r")]
        [InlineData("1.000\n2.000\n")]
        public void BuildModel_GivenStringContainsDecimals_CategoryValuesShouldBePopulation(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            result.ElementAt(0).Tonnage.Should().Be("1.000");
            result.ElementAt(1).Tonnage.Should().Be("2.000");
        }

        [Theory]
        [InlineData("\r\n")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void BuildModel_GivenStringContainsSingleNewlines_CategoryValuesShouldNotBePopulated(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertEmptyValues(result);
        }

        [Theory]
        [InlineData("\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("\r\r\r\r\r\r\r\r\r\r\r\r\r\r\r\r")]
        [InlineData("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n")]
        public void BuildModel_GivenStringContainsAllNewlines_CategoryValuesShouldNotBePopulated(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertEmptyValues(result);
        }

        [Theory]
        [InlineData("1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n10\r\n11\r\n12\r\n13\r\n14")]
        [InlineData("1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14")]
        [InlineData("1\r2\r3\r4\r5\r6\r7\r8\r9\r10\r11\r12\r13\r14")]
        public void BuildModel_GivenStringContainsOneColumnAndFourteenRows_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertPopulatedValues(result);
        }

        [Theory]
        [InlineData("1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n10\r\n11\r\n12\r\n13\r\n14\r\n15\r\n16")]
        [InlineData("1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\n15\n16")]
        [InlineData("1\r2\r3\r4\r5\r6\r7\r8\r9\r10\r11\r12\r13\r14\r15\r16")]
        public void BuildModel_GivenStringContainsOneColumnAndMoreThanFourteenRows_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertPopulatedValues(result);
        }

        [Theory]
        [InlineData("1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7")]
        [InlineData("1\n2\n3\n4\n5\n6\n7")]
        [InlineData("1\r2\r3\r4\r5\r6\r7")]
        public void BuildModel_GivenStringContainsOneColumnAndLessThanFourteenRows_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertHalfPopulatedValues(result);
        }

        [Theory]
        [InlineData("1\t15\r\n2\t16\r\n3\t17\r\n4\t18\r\n5\t19\r\n6\t20\r\n7\t21\r\n8\t22\r\n9\t23\r\n10\t24\r\n11\t25\r\n12\t26\r\n13\t27\r\n14\t28")]
        [InlineData("1\t15\r2\t16\r3\t17\r4\t18\r5\t19\r6\t20\r7\t21\r8\t22\r9\t23\r10\t24\r11\t25\r12\t26\r13\t27\r14\t28")]
        [InlineData("1\t15\n2\t16\n3\t17\n4\t18\n5\t19\n6\t20\n7\t21\n8\t22\n9\t23\n10\t24\n11\t25\n12\t26\n13\t27\n14\t28")]
        public void BuildModel_GivenStringContainsTwoColumns_CategoryValuesShouldBePopulatedOnlyForFirstColumn(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertPopulatedValues(result);
        }

        [Theory]
        [InlineData("1\t15\r\n2\t16\r\n3\t17\r\n4\t18\r\n5\t19\r\n6\t20\r\n7\t21")]
        [InlineData("1\t15\n2\t16\n3\t17\n4\t18\n5\t19\n6\t20\n7\t21")]
        [InlineData("1\t15\n2\t16\n3\t17\n4\t18\n5\t19\n6\t20\n7\t21")]
        public void BuildModel_GivenStringContainsTwoColumnsAndLessThanFourteenRows_CategoryValuesShouldBePopulatedOnlyForFirstColumn(string value)
        {
            var result = pasteProcessor.BuildModel(value);

            AssertHalfPopulatedValues(result);
        }

        [Fact]
        public void ParseObligatedPastedValues_GivenPastedDataButNoExistingData_ObligatedCategoryValuesShouldBeCorrectlyPopulated()
        {
            var obligatedPastedValues = CreateObligatedPastedData(out var ignored);

            var results = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues, null);

            foreach (var result in results)
            {
                result.B2B.Should().Be(obligatedPastedValues.B2B.FirstOrDefault(s => s.CategoryId == result.CategoryId).Tonnage);
                result.B2C.Should().Be(obligatedPastedValues.B2C.FirstOrDefault(s => s.CategoryId == result.CategoryId).Tonnage);
            }
        }

        [Fact]
        public void ParseObligatedPastedValues_GivenPastedDataAndExistingData_ObligatedCategoryValuesShouldBeCorrectlyPopulated()
        {
            var obligatedPastedValues = CreateObligatedPastedData(out var existingData);

            var results = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues, existingData);

            foreach (var result in results)
            {
                result.Id.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).Id);
                result.B2B.Should().Be(obligatedPastedValues.B2B.FirstOrDefault(s => s.CategoryId == result.CategoryId).Tonnage);
                result.B2C.Should().Be(obligatedPastedValues.B2C.FirstOrDefault(s => s.CategoryId == result.CategoryId).Tonnage);
            }
        }

        [Fact]
        public void ParseObligatedPastedValues_GivenPastedDataAndB2bColumnOnlyOfExistingData_ObligatedCategoryValuesShouldBeCorrectlyPopulatedAndB2cShouldBeKept()
        {
            var obligatedPastedValues = CreateObligatedPastedData(out var existingData, includeb2c: false);

            var results = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues, existingData);

            foreach (var result in results)
            {
                result.Id.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).Id);
                result.B2B.Should().Be(obligatedPastedValues.B2B.FirstOrDefault(s => s.CategoryId == result.CategoryId).Tonnage);
                result.B2C.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).B2C);
            }
        }

        [Fact]
        public void ParseObligatedPastedValues_GivenPastedDataAndB2cColumnOnlyOfExistingData_ObligatedCategoryValuesShouldBeCorrectlyPopulatedAndB2bShouldBeKept()
        {
            var obligatedPastedValues = CreateObligatedPastedData(out var existingData, includeb2b: false);

            var results = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues, existingData);

            results.Select(r => r.B2B).Count().Should().Be(existingData.Select(e => e.B2B).Count());
            foreach (var result in results)
            {
                result.Id.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).Id);
                result.B2B.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).B2B);
                result.B2C.Should().Be(obligatedPastedValues.B2C.FirstOrDefault(s => s.CategoryId == result.CategoryId).Tonnage);
            }
        }

        [Fact]
        public void ParseObligatedPastedValues_GivenExistingDataButNoPastedData_NoChangesShouldBeMade()
        {
            var obligatedPastedValues = CreateObligatedPastedData(out var existingData, includeb2b: false, includeb2c: false);

            var results = pasteProcessor.ParseObligatedPastedValues(obligatedPastedValues, existingData);

            results.Select(r => r.B2B).Count().Should().Be(existingData.Select(e => e.B2B).Count());
            foreach (var result in results)
            {
                result.Id.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).Id);
                result.B2B.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).B2B);
                result.B2C.Should().Be(existingData.FirstOrDefault(s => s.CategoryId == result.CategoryId).B2C);
            }
        }

        [Fact]
        public void ParseNonObligatedPastedValues_GivenPastedDataButNoExistingData_NonObligatedCategoryValuesShouldBeCorrectlyPopulated()
        {
            PastedValues pastedValues = CreateNonObligatedPastedData();

            var result = pasteProcessor.ParseNonObligatedPastedValues(pastedValues, null);

            for (var i = 0; i < result.Count; i++)
            {
                result[i].Tonnage.Should().Be(i.ToString());
            }
        }

        [Fact]
        public void ParseNonObligatedPastedValues_GivenPastedDataAndExistingData_NonObligatedCategoryValuesShouldBeCorrectlyPopulated()
        {
            PastedValues pastedValues = CreateNonObligatedPastedData();

            var existingData = A.Fake<NonObligatedCategoryValues>();

            foreach (var item in existingData)
            {
                item.Id = Guid.NewGuid();
            }

            var results = pasteProcessor.ParseNonObligatedPastedValues(pastedValues, existingData);

            foreach (var result in results)
            {
                result.Id.Should().Be(existingData.Where(s => s.CategoryId == result.CategoryId).FirstOrDefault().Id);
            }
        }

        [Fact]
        public void ParseEvidencePastedValues_GivenPastedDataAndExistingData_EvidenceCategoryValuesShouldBeCorrectlyPopulated()
        {
            //Arrange
            var existingData = A.Fake<IList<EvidenceCategoryValue>>();
            var evidencePastedValues = CreateEvidencePastedData();

            foreach (var item in existingData)
            {
                item.Id = Guid.NewGuid();
            }

            //Act
            var results = pasteProcessor.ParseEvidencePastedValues(evidencePastedValues, existingData);

            //Assert
            foreach (var result in results)
            {
                result.Id.Should().Be(existingData.Where(s => s.CategoryId == result.CategoryId).FirstOrDefault().Id);
            }
        }

        [Fact]
        public void ParseEvidencePastedValues_GivenPastedDataAndNoExistingData_EvidenceCategoryValuesShouldBeCorrectlyPopulated()
        {
            //Arrange
            var evidencePastedValues = CreateEvidencePastedData();

            //Act
            var result = pasteProcessor.ParseEvidencePastedValues(evidencePastedValues, null);

            //Assert
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Received.Should().Be(evidencePastedValues.Receieved.FirstOrDefault(x => x.CategoryId == result[i].CategoryId).Tonnage);
                result[i].Reused.Should().Be(evidencePastedValues.Reused.FirstOrDefault(x => x.CategoryId == result[i].CategoryId).Tonnage);
            }
        }

        private ObligatedPastedValues CreateObligatedPastedData(out ObligatedCategoryValues existingData, bool includeb2b = true, bool includeb2c = true)
        {
            var pastedB2bValues = new PastedValues();
            var pastedB2cValues = new PastedValues();
            existingData = new ObligatedCategoryValues();

            for (var i = 0; i < pastedB2bValues.Count; i++)
            {
                if (includeb2b)
                {
                    pastedB2bValues[i].Tonnage = fixture.Create<int>().ToString();
                }

                if (includeb2c)
                {
                    pastedB2cValues[i].Tonnage = fixture.Create<int>().ToString();
                }

                var existing = existingData.FirstOrDefault(e => e.CategoryId == pastedB2bValues[i].CategoryId);
                existing.Id = pastedB2bValues[i].Id;
                existing.B2B = fixture.Create<int>().ToString();
                existing.B2C = fixture.Create<int>().ToString();
            }

            return new ObligatedPastedValues
            {
                B2B = pastedB2bValues,
                B2C = pastedB2cValues
            };
        }

        private static PastedValues CreateNonObligatedPastedData()
        {
            var pastedValues = new PastedValues();

            for (var i = 0; i < pastedValues.Count; i++)
            {
                pastedValues[i].Tonnage = i.ToString();
            }

            return pastedValues;
        }

        private EvidencePastedValues CreateEvidencePastedData()
        {
            var evidencePastedValues = new EvidencePastedValues();
            var receivedPastedValues = new PastedValues();
            var reusedPastedValues = new PastedValues();

            for (var i = 0; i < receivedPastedValues.Count; i++)
            {
                receivedPastedValues[i].Tonnage = fixture.Create<int>().ToString();
                reusedPastedValues[i].Tonnage = fixture.Create<int>().ToString();
            }

            evidencePastedValues.Reused = reusedPastedValues;
            evidencePastedValues.Receieved = receivedPastedValues;

            return evidencePastedValues;
        }

        private static void AssertEmptyValues(PastedValues result)
        {
            result.Count(c => c.Tonnage != null).Should().Be(0);
        }

        private static void AssertPopulatedValues(PastedValues result)
        {
            result.ElementAt(0).Tonnage.Should().Be("1");
            result.ElementAt(1).Tonnage.Should().Be("2");
            result.ElementAt(2).Tonnage.Should().Be("3");
            result.ElementAt(3).Tonnage.Should().Be("4");
            result.ElementAt(4).Tonnage.Should().Be("5");
            result.ElementAt(5).Tonnage.Should().Be("6");
            result.ElementAt(6).Tonnage.Should().Be("7");
            result.ElementAt(7).Tonnage.Should().Be("8");
            result.ElementAt(8).Tonnage.Should().Be("9");
            result.ElementAt(9).Tonnage.Should().Be("10");
            result.ElementAt(10).Tonnage.Should().Be("11");
            result.ElementAt(11).Tonnage.Should().Be("12");
            result.ElementAt(12).Tonnage.Should().Be("13");
            result.ElementAt(13).Tonnage.Should().Be("14");
            result.Count.Should().Be(14);
        }

        private static void AssertHalfPopulatedValues(PastedValues result)
        {
            result.ElementAt(0).Tonnage.Should().Be("1");
            result.ElementAt(1).Tonnage.Should().Be("2");
            result.ElementAt(2).Tonnage.Should().Be("3");
            result.ElementAt(3).Tonnage.Should().Be("4");
            result.ElementAt(4).Tonnage.Should().Be("5");
            result.ElementAt(5).Tonnage.Should().Be("6");
            result.ElementAt(6).Tonnage.Should().Be("7");
            result.ElementAt(7).Tonnage.Should().Be(null);
            result.ElementAt(8).Tonnage.Should().Be(null);
            result.ElementAt(9).Tonnage.Should().Be(null);
            result.ElementAt(10).Tonnage.Should().Be(null);
            result.ElementAt(11).Tonnage.Should().Be(null);
            result.ElementAt(12).Tonnage.Should().Be(null);
            result.ElementAt(13).Tonnage.Should().Be(null);
            result.Count.Should().Be(14);
        }
    }
}
