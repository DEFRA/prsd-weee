namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
