namespace EA.Prsd.Email
{
    public interface ITemplateExecutor
    {
        string Execute(string name, object model);
    }
}