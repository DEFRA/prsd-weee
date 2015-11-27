namespace EA.Weee.Core.Search.Fuzzy
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines part of a search phrase or a result phrase.
    /// A single term may have many equivalent values.
    /// </summary>
    public interface ITerm
    {
        IEnumerable<string> Values { get; }
    }
}
