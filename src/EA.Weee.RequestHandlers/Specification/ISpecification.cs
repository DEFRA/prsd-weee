namespace EA.Weee.RequestHandlers.Specification
{
    using System;
    using System.Linq.Expressions;
    using Prsd.Core.Domain;

    public interface ISpecification<T> where T : Entity
    {
        Expression<Func<T, bool>> ToExpression();
    }
}
