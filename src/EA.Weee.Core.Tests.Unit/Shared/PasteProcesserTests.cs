namespace EA.Weee.Core.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Shared;
    using FluentAssertions;
    using Xunit;

    public class PasteProcesserTests
    {
        private readonly PasteProcesser pasteProcesser;

        public PasteProcesserTests()
        {
            pasteProcesser = new PasteProcesser();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BuildModel_GivenNullString_EmptyCategoriesModelExpected(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.Should().BeOfType<ObligatedCategoryValues>();
            AssertEmptyValues(result);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1\r\n")]
        [InlineData("1\r")]
        [InlineData("1\n")]
        public void BuildModel_GivenStringContainsSingleNumericValue_CategoryValuesShouldBeOne(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.ElementAt(0).B2C.Should().Be("1");
        }

        [Theory]
        [InlineData("1\r\n2\r\n")]
        [InlineData("1\r2\r")]
        [InlineData("1\n2\n")]
        public void BuildModel_GivenStringContainsOneColumnsAndTwoRows_CategoryValuesShouldBePopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.ElementAt(0).B2C.Should().Be("1");
            result.ElementAt(1).B2C.Should().Be("2");
        }

        [Theory]
        [InlineData("\r\n")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void BuildModel_GivenStringContainsSingleNewlines_CategoryValuesShouldNotBePopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            AssertEmptyValues(result);
        }

        [Theory]
        [InlineData("\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("\r\r\r\r\r\r\r\r\r\r\r\r\r\r\r\r")]
        [InlineData("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n")]
        public void BuildModel_GivenStringContainsAllNewlines_CategoryValuesShouldNotBePopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            AssertEmptyValues(result);
        }

        [Theory]
        [InlineData("1\t2\r\n")]
        [InlineData("1\t2\r")]
        [InlineData("1\t2\n")]
        [InlineData("1:2\r\n")]
        [InlineData("1:2\r")]
        [InlineData("1:2\n")]
        [InlineData("1,2\r\n")]
        [InlineData("1,2\r")]
        [InlineData("1,2\n")]
        public void BuildModel_GivenStringContainsTwoColumnsAndOneRow_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.ElementAt(0).B2C.Should().Be("1");
            result.ElementAt(0).B2B.Should().Be("2");
        }

        [Theory]
        [InlineData("1\t2\r\n3\t4\r\n5\t6\r\n7\t8\r\n9\t10\r\n11\t12\r\n13\t14\r\n15\t16\r\n17\t18\r\n19\t20\r\n21\t22\r\n23\t24\r\n25\t26\r\n27\t28\r\n")]
        [InlineData("1\t2\r3\t4\r5\t6\r7\t8\r9\t10\r11\t12\r13\t14\r15\t16\r17\t18\r19\t20\r21\t22\r23\t24\r25\t26\r27\t28\r")]
        [InlineData("1\t2\n3\t4\n5\t6\n7\t8\n9\t10\n11\t12\n13\t14\n15\t16\n17\t18\n19\t20\n21\t22\n23\t24\n25\t26\n27\t28\n")]
        [InlineData("1:2\r\n3:4\r\n5:6\r\n7:8\r\n9:10\r\n11:12\r\n13:14\r\n15:16\r\n17:18\r\n19:20\r\n21:22\r\n23:24\r\n25:26\r\n27:28\r\n")]
        [InlineData("1:2\r3:4\r5:6\r7:8\r9:10\r11:12\r13:14\r15:16\r17:18\r19:20\r21:22\r23:24\r25:26\r27:28\r")]
        [InlineData("1:2\n3:4\n5:6\n7:8\n9:10\n11:12\n13:14\n15:16\n17:18\n19:20\n21:22\n23:24\n25:26\n27:28\n")]
        [InlineData("1,2\r\n3,4\r\n5,6\r\n7,8\r\n9,10\r\n11,12\r\n13,14\r\n15,16\r\n17,18\r\n19,20\r\n21,22\r\n23,24\r\n25,26\r\n27,28\r\n")]
        [InlineData("1,2\r3,4\r5,6\r7,8\r9,10\r11,12\r13,14\r15,16\r17,18\r19,20\r21,22\r23,24\r25,26\r27,28\r")]
        [InlineData("1,2\n3,4\n5,6\n7,8\n9,10\n11,12\n13,14\n15,16\n17,18\n19,20\n21,22\n23,24\n25,26\n27,28\n")]
        public void BuildModel_GivenStringContainsTwoColumnsAndFourteenRows_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            AssertPopulatedValues(result);
        }

        [Theory]
        [InlineData("1\t2\r\n3\t4\r\n\r\n5\t6\r\n7\t8\r\n9\t10\r\n11\t12\r\n13\t14\r\n\r\n15\t16\r\n17\t18\r\n19\t20\r\n21\t22\r\n23\t24\r\n25\t26\r\n27\t28\r\n\r\n\r\n\r\n")]
        [InlineData("1\t2\r3\t4\r\r5\t6\r7\t8\r9\t10\r11\t12\r13\t14\r\r15\t16\r17\t18\r19\t20\r21\t22\r23\t24\r25\t26\r27\t28\r\r\r\r")]
        [InlineData("1\t2\n3\t4\n\n5\t6\n7\t8\n9\t10\n11\t12\n13\t14\n\n15\t16\n17\t18\n19\t20\n21\t22\n23\t24\n25\t26\n27\t28\n\n\n\n")]
        [InlineData("1:2\r\n3:4\r\n\r\n5:6\r\n7:8\r\n9:10\r\n11:12\r\n13:14\r\n\r\n15:16\r\n17:18\r\n19:20\r\n21:22\r\n23:24\r\n25:26\r\n27:28\r\n\r\n\r\n\r\n")]
        [InlineData("1:2\r3:4\r\r5:6\r7:8\r9:10\r11:12\r13:14\r\r15:16\r17:18\r19:20\r21:22\r23:24\r25:26\r27:28\r\r\r\r")]
        [InlineData("1:2\n3:4\n\n5:6\n7:8\n9:10\n11:12\n13:14\n\n15:16\n17:18\n19:20\n21:22\n23:24\n25:26\n27:28\n\n\n\n")]
        [InlineData("1,2\r\n3,4\r\n\r\n5,6\r\n7,8\r\n9,10\r\n11,12\r\n13,14\r\n\r\n15,16\r\n17,18\r\n19,20\r\n21,22\r\n23,24\r\n25,26\r\n27,28\r\n\r\n\r\n\r\n")]
        [InlineData("1,2\r3,4\r\r5,6\r7,8\r9,10\r11,12\r13,14\r\r15,16\r17,18\r19,20\r21,22\r23,24\r25,26\r27,28\r\r\r\r")]
        [InlineData("1,2\n3,4\n\n5,6\n7,8\n9,10\n11,12\n13,14\n\n15,16\n17,18\n19,20\n21,22\n23,24\n25,26\n27,28\n\n\n\n")]
        public void BuildModel_GivenStringContainsTwoColumnsAndFourteenRowsWithSpuriousNewlines_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.ElementAt(0).B2C.Should().Be("1");
            result.ElementAt(0).B2B.Should().Be("2");
            result.ElementAt(1).B2C.Should().Be("3");
            result.ElementAt(1).B2B.Should().Be("4");
            result.ElementAt(2).B2C.Should().BeNull();
            result.ElementAt(2).B2B.Should().BeNull();
            result.ElementAt(3).B2C.Should().Be("5");
            result.ElementAt(3).B2B.Should().Be("6");
            result.ElementAt(4).B2C.Should().Be("7");
            result.ElementAt(4).B2B.Should().Be("8");
            result.ElementAt(5).B2C.Should().Be("9");
            result.ElementAt(5).B2B.Should().Be("10");
            result.ElementAt(6).B2C.Should().Be("11");
            result.ElementAt(6).B2B.Should().Be("12");
            result.ElementAt(7).B2C.Should().Be("13");
            result.ElementAt(7).B2B.Should().Be("14");
            result.ElementAt(8).B2C.Should().BeNull();
            result.ElementAt(8).B2B.Should().BeNull();
            result.ElementAt(9).B2C.Should().Be("15");
            result.ElementAt(9).B2B.Should().Be("16");
            result.ElementAt(10).B2C.Should().Be("17");
            result.ElementAt(10).B2B.Should().Be("18");
            result.ElementAt(11).B2C.Should().Be("19");
            result.ElementAt(11).B2B.Should().Be("20");
            result.ElementAt(12).B2C.Should().Be("21");
            result.ElementAt(12).B2B.Should().Be("22");
            result.ElementAt(13).B2C.Should().Be("23");
            result.ElementAt(13).B2B.Should().Be("24");
            result.Count.Should().Be(14);
        }

        [Theory]
        [InlineData("1\t2\r\n3\t4\r\n5\t6\r\n7\t8\r\n9\t10\r\n11\t12\r\n13\t14\r\n15\t16\r\n17\t18\r\n19\t20\r\n21\t22\r\n23\t24\r\n25\t26\r\n27\t28\r\n29\t30\r\n")]
        [InlineData("1\t2\r3\t4\r5\t6\r7\t8\r9\t10\r11\t12\r13\t14\r15\t16\r17\t18\r19\t20\r21\t22\r23\t24\r25\t26\r27\t28\r29\t30\r")]
        [InlineData("1\t2\n3\t4\n5\t6\n7\t8\n9\t10\n11\t12\n13\t14\n15\t16\n17\t18\n19\t20\n21\t22\n23\t24\n25\t26\n27\t28\n29\t30\n")]
        [InlineData("1:2\r\n3:4\r\n5:6\r\n7:8\r\n9:10\r\n11:12\r\n13:14\r\n15:16\r\n17:18\r\n19:20\r\n21:22\r\n23:24\r\n25:26\r\n27:28\r\n29:30\r\n")]
        [InlineData("1:2\r3:4\r5:6\r7:8\r9:10\r11:12\r13:14\r15:16\r17:18\r19:20\r21:22\r23:24\r25:26\r27:28\r29:30\r")]
        [InlineData("1:2\n3:4\n5:6\n7:8\n9:10\n11:12\n13:14\n15:16\n17:18\n19:20\n21:22\n23:24\n25:26\n27:28\n29:30\n")]
        [InlineData("1,2\r\n3,4\r\n5,6\r\n7,8\r\n9,10\r\n11,12\r\n13,14\r\n15,16\r\n17,18\r\n19,20\r\n21,22\r\n23,24\r\n25,26\r\n27,28\r\n29,30\r\n")]
        [InlineData("1,2\r3,4\r5,6\r7,8\r9,10\r11,12\r13,14\r15,16\r17,18\r19,20\r21,22\r23,24\r25,26\r27,28\r29,30\r")]
        [InlineData("1,2\n3,4\n5,6\n7,8\n9,10\n11,12\n13,14\n15,16\n17,18\n19,20\n21,22\n23,24\n25,26\n27,28\n29,30\n")]
        public void BuildModel_GivenStringContainsTwoColumnsAndMoreThanFourteenRows_CategoryValuesShouldPopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            AssertPopulatedValues(result);
        }

        [Theory]
        [InlineData("1\t2\t3\t4\r\n5\t6\t7\r\n")]
        [InlineData("1:2:3:4\r\n5:6:7\r\n")]
        [InlineData("1,2,3,4\r\n5,6,7\r\n")]
        public void BuildModel_GivenStringContainsRowsWithMoreThenTwoColumns_CategoryValueShouldBePopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.ElementAt(0).B2C.Should().Be("1");
            result.ElementAt(0).B2B.Should().Be("2");
            result.ElementAt(1).B2C.Should().Be("5");
            result.ElementAt(1).B2B.Should().Be("6");
            result.Count(c => c.B2C != null).Should().Be(2);
            result.Count(c => c.B2B != null).Should().Be(2);
        }

        private static void AssertEmptyValues(ObligatedCategoryValues result)
        {
            result.Count(c => c.B2C != null).Should().Be(0);
            result.Count(c => c.B2B != null).Should().Be(0);
        }

        private static void AssertPopulatedValues(ObligatedCategoryValues result)
        {
            result.ElementAt(0).B2C.Should().Be("1");
            result.ElementAt(0).B2B.Should().Be("2");
            result.ElementAt(1).B2C.Should().Be("3");
            result.ElementAt(1).B2B.Should().Be("4");
            result.ElementAt(2).B2C.Should().Be("5");
            result.ElementAt(2).B2B.Should().Be("6");
            result.ElementAt(3).B2C.Should().Be("7");
            result.ElementAt(3).B2B.Should().Be("8");
            result.ElementAt(4).B2C.Should().Be("9");
            result.ElementAt(4).B2B.Should().Be("10");
            result.ElementAt(5).B2C.Should().Be("11");
            result.ElementAt(5).B2B.Should().Be("12");
            result.ElementAt(6).B2C.Should().Be("13");
            result.ElementAt(6).B2B.Should().Be("14");
            result.ElementAt(7).B2C.Should().Be("15");
            result.ElementAt(7).B2B.Should().Be("16");
            result.ElementAt(8).B2C.Should().Be("17");
            result.ElementAt(8).B2B.Should().Be("18");
            result.ElementAt(9).B2C.Should().Be("19");
            result.ElementAt(9).B2B.Should().Be("20");
            result.ElementAt(10).B2C.Should().Be("21");
            result.ElementAt(10).B2B.Should().Be("22");
            result.ElementAt(11).B2C.Should().Be("23");
            result.ElementAt(11).B2B.Should().Be("24");
            result.ElementAt(12).B2C.Should().Be("25");
            result.ElementAt(12).B2B.Should().Be("26");
            result.ElementAt(13).B2C.Should().Be("27");
            result.ElementAt(13).B2B.Should().Be("28");
            result.Count.Should().Be(14);
        }
    }
}
