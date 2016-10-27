namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;

    public interface ICsvWriter<T>
    {
        IEnumerable<string> ColumnTitles { get; }

        void DefineColumn(string title, Func<T, object> func, bool formatAsText = false);

        string Write(IEnumerable<T> items);
    }
}