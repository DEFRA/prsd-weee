namespace EA.Weee.Xml.Converter
{
    /// <summary>
    /// Traverses an object graph to find all string properties with attributes identifying them with the XML
    /// data type of "Token". Any such properties are tokenised according to the rules defined by the XML 
    /// standard for collapsing whitespace.
    /// </summary>
    public interface IWhiteSpaceCollapser
    {
        void Collapse(object @object);
    }
}
