namespace EA.Weee.Core.XmlBusinessValidation
{
    public interface IRule<in T>
    {
        /// <summary>
        /// Evaluates the rule
        /// </summary>
        /// <param name="ruleData">The data required to evaluate the rule</param>
        /// <returns>A boolean indicating whether the specified rule is adhered to or not</returns>
        bool Evaluate(T ruleData);
    }
}
