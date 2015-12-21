namespace EA.Weee.Xml.Tests.Unit.Converter
{
    using Xml.Converter;
    using Xunit;

    public class WhiteSpaceCollapserTests
    {
        /// <summary>
        /// This test ensures the white space collapser will replace new lines with single spaces.
        /// </summary>
        [Fact]
        public void CollpaseWhiteSpace_ForStringWithNewLine_ReplacesNewLineWithSingleSpace()
        {
            // Arrange
            string value = @"Lorem ipsum
dolor sit amet,
consectetur adipiscing

elit.";

            // Act
            WhiteSpaceCollapser collapser = new WhiteSpaceCollapser();

            string result = collapser.CollapseWhiteSpace(value);

            // Asert
            Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", result);
        }

        /// <summary>
        /// This test ensures the white space collapser will replace tabs with single spaces.
        /// </summary>
        [Fact]
        public void CollpaseWhiteSpace_ForStringWithTabs_ReplacesTabsWithSingleSpace()
        {
            // Arrange
            char tab = (char)9;
            string value = @"Lorem ipsum" + tab + "dolor sit amet," + tab + tab + "consectetur adipiscing elit.";

            // Act
            WhiteSpaceCollapser collapser = new WhiteSpaceCollapser();

            string result = collapser.CollapseWhiteSpace(value);

            // Asert
            Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", result);
        }

        /// <summary>
        /// This test ensures the white space collapser will replace runs of spaces with single spaces.
        /// </summary>
        [Fact]
        public void CollpaseWhiteSpace_ForStringWithRunsOfSpaces_ReplacesSpacesWithSingleSpace()
        {
            // Arrange
            string value = @"Lorem     ipsum dolor     sit amet,     consectetur adipiscing elit.";

            // Act
            WhiteSpaceCollapser collapser = new WhiteSpaceCollapser();

            string result = collapser.CollapseWhiteSpace(value);

            // Asert
            Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", result);
        }

        /// <summary>
        /// This test ensures the white space collapser will remove leading/trailing whitespace.
        /// </summary>
        [Fact]
        public void CollpaseWhiteSpace_ForStringWithLeadingAndTrailingWhitespace_RemovesLeadingAndTrailingWhitespace()
        {
            // Arrange
            string value = @"       Lorem ipsum dolor sit amet, consectetur adipiscing elit.       ";

            // Act
            WhiteSpaceCollapser collapser = new WhiteSpaceCollapser();

            string result = collapser.CollapseWhiteSpace(value);

            // Asert
            Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", result);
        }
    }
}
