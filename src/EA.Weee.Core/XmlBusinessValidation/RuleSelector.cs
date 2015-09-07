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

        public RuleResult EvaluateRule<T>(T ruleData)
        {
            IRule<T> rule;

            try
            {
                rule = (IRule<T>)context.Resolve(typeof(IRule<>).MakeGenericType(typeof(T)));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("No rule evaluator found for rule '{0}'", typeof(T).Name), ex);
            }

            return rule.Evaluate(ruleData);
        }
    }
}
