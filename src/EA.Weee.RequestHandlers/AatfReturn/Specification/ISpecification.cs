namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Prsd.Core.Domain;
    using System;
    using System.Linq.Expressions;

    public interface ISpecification<T> where T : Entity
    {
        Expression<Func<T, bool>> ToExpression();
    }
}
