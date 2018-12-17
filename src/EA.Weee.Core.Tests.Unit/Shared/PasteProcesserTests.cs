namespace EA.Weee.Core.Tests.Unit.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
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

            result.Should().BeOfType<CategoryValues>();
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

            result.ElementAt(0).HouseHold.Should().Be("1");
        }

        [Theory]
        [InlineData("1\r\n2\r\n")]
        [InlineData("1\r2\r")]
        [InlineData("1\n2\n")]
        public void BuildModel_GivenStringContainsOneColumnsAndTwoRows_CategoryValuesShouldBePopulated(string value)
        {
            var result = pasteProcesser.BuildModel(value);

            result.ElementAt(0).HouseHold.Should().Be("1");
            result.ElementAt(1).HouseHold.Should().Be("2");
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

            result.ElementAt(0).HouseHold.Should().Be("1");
            result.ElementAt(0).NonHouseHold.Should().Be("2");
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

            result.ElementAt(0).HouseHold.Should().Be("1");
            result.ElementAt(0).NonHouseHold.Should().Be("2");
            result.ElementAt(1).HouseHold.Should().Be("3");
            result.ElementAt(1).NonHouseHold.Should().Be("4");
            result.ElementAt(2).HouseHold.Should().BeNull();
            result.ElementAt(2).NonHouseHold.Should().BeNull();
            result.ElementAt(3).HouseHold.Should().Be("5");
            result.ElementAt(3).NonHouseHold.Should().Be("6");
            result.ElementAt(4).HouseHold.Should().Be("7");
            result.ElementAt(4).NonHouseHold.Should().Be("8");
            result.ElementAt(5).HouseHold.Should().Be("9");
            result.ElementAt(5).NonHouseHold.Should().Be("10");
            result.ElementAt(6).HouseHold.Should().Be("11");
            result.ElementAt(6).NonHouseHold.Should().Be("12");
            result.ElementAt(7).HouseHold.Should().Be("13");
            result.ElementAt(7).NonHouseHold.Should().Be("14");
            result.ElementAt(8).HouseHold.Should().BeNull();
            result.ElementAt(8).NonHouseHold.Should().BeNull();
            result.ElementAt(9).HouseHold.Should().Be("15");
            result.ElementAt(9).NonHouseHold.Should().Be("16");
            result.ElementAt(10).HouseHold.Should().Be("17");
            result.ElementAt(10).NonHouseHold.Should().Be("18");
            result.ElementAt(11).HouseHold.Should().Be("19");
            result.ElementAt(11).NonHouseHold.Should().Be("20");
            result.ElementAt(12).HouseHold.Should().Be("21");
            result.ElementAt(12).NonHouseHold.Should().Be("22");
            result.ElementAt(13).HouseHold.Should().Be("23");
            result.ElementAt(13).NonHouseHold.Should().Be("24");
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

        //[Fact]
        //public void BuildModel_GivenStringContainsNoNumericCharactors_CategoryValuesShouldBeNull()
        //{
        //    var result = pasteProcesser.BuildModel(@"!\t\n""\t£\n$\t%\n^\t^\n&\t*\n(\t)\n-\t_\n=\t+\n[\t]\n@\t~\n#\t?\n|\t¬\n.\t<\n>\t.\n!\t\n");

        //    AssertEmptyValues(result);
        //}
        // what about string that just contains newline or spacing char       

        // test for empty lines 

        // too many values pasted into the field / do a test for this

        // strip empty

        // split string by newline 

        // split each newline by spacing char

        private static void AssertEmptyValues(CategoryValues result)
        {
            result.Count(c => c.HouseHold != null).Should().Be(0);
            result.Count(c => c.NonHouseHold != null).Should().Be(0);
        }

        private static void AssertPopulatedValues(CategoryValues result)
        {
            result.ElementAt(0).HouseHold.Should().Be("1");
            result.ElementAt(0).NonHouseHold.Should().Be("2");
            result.ElementAt(1).HouseHold.Should().Be("3");
            result.ElementAt(1).NonHouseHold.Should().Be("4");
            result.ElementAt(2).HouseHold.Should().Be("5");
            result.ElementAt(2).NonHouseHold.Should().Be("6");
            result.ElementAt(3).HouseHold.Should().Be("7");
            result.ElementAt(3).NonHouseHold.Should().Be("8");
            result.ElementAt(4).HouseHold.Should().Be("9");
            result.ElementAt(4).NonHouseHold.Should().Be("10");
            result.ElementAt(5).HouseHold.Should().Be("11");
            result.ElementAt(5).NonHouseHold.Should().Be("12");
            result.ElementAt(6).HouseHold.Should().Be("13");
            result.ElementAt(6).NonHouseHold.Should().Be("14");
            result.ElementAt(7).HouseHold.Should().Be("15");
            result.ElementAt(7).NonHouseHold.Should().Be("16");
            result.ElementAt(8).HouseHold.Should().Be("17");
            result.ElementAt(8).NonHouseHold.Should().Be("18");
            result.ElementAt(9).HouseHold.Should().Be("19");
            result.ElementAt(9).NonHouseHold.Should().Be("20");
            result.ElementAt(10).HouseHold.Should().Be("21");
            result.ElementAt(10).NonHouseHold.Should().Be("22");
            result.ElementAt(11).HouseHold.Should().Be("23");
            result.ElementAt(11).NonHouseHold.Should().Be("24");
            result.ElementAt(12).HouseHold.Should().Be("25");
            result.ElementAt(12).NonHouseHold.Should().Be("26");
            result.ElementAt(13).HouseHold.Should().Be("27");
            result.ElementAt(13).NonHouseHold.Should().Be("28");
            result.Count.Should().Be(14);
        }
    }
}
