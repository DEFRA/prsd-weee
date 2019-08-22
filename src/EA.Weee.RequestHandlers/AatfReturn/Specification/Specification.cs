namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Prsd.Core.Domain;
    using System;
    using System.Linq.Expressions;

    public abstract class Specification<T> : ISpecification<T> where T : Entity
    {
        public abstract Expression<Func<T, bool>> ToExpression();
    }
}
