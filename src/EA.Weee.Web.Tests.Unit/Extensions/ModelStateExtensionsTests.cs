namespace EA.Weee.Web.Tests.Unit.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Web.Extensions;
    using Xunit;

    public class ModelStateExtensionsTests
    {
        [Fact]
        public void ApplyCustomValidationSummaryOrdering_AllRequested_ShouldOrderAsRequested()
        {
            var first = "First";
            var second = "Second";
            var third = "Third";
            var last = "Last";
            var firstPair = new KeyValuePair<string, ModelState>(first, null);
            var secondPair = new KeyValuePair<string, ModelState>(second, null);
            var thirdPair = new KeyValuePair<string, ModelState>(third, null);
            var fourthPair = new KeyValuePair<string, ModelState>(last, null);

            var modelState = new ModelStateDictionary();
            modelState.Add(thirdPair);
            modelState.Add(secondPair);
            modelState.Add(fourthPair);
            modelState.Add(firstPair);

            modelState.ApplyCustomValidationSummaryOrdering(new List<string> { first, second, third, last });

            Assert.Equal(4, modelState.Count);
            Assert.Equal(firstPair, modelState.First());
            Assert.Equal(secondPair, modelState.Skip(1).First());
            Assert.Equal(thirdPair, modelState.Skip(2).First());
            Assert.Equal(fourthPair, modelState.Last());
        }

        [Fact]
        public void ApplyCustomValidationSummaryOrdering_OneNotRequested_ShouldShowMissingLast()
        {
            var first = "First";
            var second = "Second";
            var third = "Third";
            var missing = "Missing";
            var firstPair = new KeyValuePair<string, ModelState>(first, null);
            var secondPair = new KeyValuePair<string, ModelState>(second, null);
            var thirdPair = new KeyValuePair<string, ModelState>(third, null);
            var missingPair = new KeyValuePair<string, ModelState>(missing, null);

            var modelState = new ModelStateDictionary();
            modelState.Add(thirdPair);
            modelState.Add(secondPair);
            modelState.Add(missingPair);
            modelState.Add(firstPair);

            modelState.ApplyCustomValidationSummaryOrdering(new List<string> { first, second, third });

            Assert.Equal(4, modelState.Count);
            Assert.Equal(firstPair, modelState.First());
            Assert.Equal(secondPair, modelState.Skip(1).First());
            Assert.Equal(thirdPair, modelState.Skip(2).First());
            Assert.Equal(missingPair, modelState.Last());
        }

        [Fact]
        public void ApplyCustomValidationSummaryOrdering_OneExtraRequested_ShouldIgnoreExtra()
        {
            var first = "First";
            var second = "Second";
            var third = "Third";
            var missing = "Missing";
            var firstPair = new KeyValuePair<string, ModelState>(first, null);
            var secondPair = new KeyValuePair<string, ModelState>(second, null);
            var thirdPair = new KeyValuePair<string, ModelState>(third, null);

            var modelState = new ModelStateDictionary();
            modelState.Add(thirdPair);
            modelState.Add(secondPair);
            modelState.Add(firstPair);

            modelState.ApplyCustomValidationSummaryOrdering(new List<string> { first, second, third, missing });

            Assert.Equal(3, modelState.Count);
            Assert.Equal(firstPair, modelState.First());
            Assert.Equal(secondPair, modelState.Skip(1).First());
            Assert.Equal(thirdPair, modelState.Skip(2).First());
        }
    }
}
