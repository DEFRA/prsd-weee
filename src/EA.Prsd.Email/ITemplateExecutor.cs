namespace EA.Prsd.Email
{
    using System;

    public interface ITemplateExecutor
    {
        string Execute(string name, object model);
    }
}
