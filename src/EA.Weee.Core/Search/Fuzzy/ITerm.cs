namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines part of a search phrase or a result phrase.
    /// A single term may have many equivalent values.
    /// </summary>
    public interface ITerm
    {
        IEnumerable<string> Values { get; }
    }
}
