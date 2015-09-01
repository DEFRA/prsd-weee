namespace EA.Weee.Core.XmlBusinessValidation
{
    using System;
    using Autofac;

    public class RuleSelector : IRuleSelector
    {
        private readonly IComponentContext context;

        public RuleSelector(IComponentContext context)
        {
            this.context = context;
        }

        public IRule<T> GetRule<T>()
        {
            try
            {
                return (IRule<T>)context.Resolve(typeof(IRule<>).MakeGenericType(typeof(T)));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("No rule found for rule data type '{0}", typeof(T).Name), ex);
            }
        }
    }
}
