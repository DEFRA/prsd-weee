namespace EA.Weee.DataAccess.Specification
{
    using System;
    using System.Linq.Expressions;
    using Prsd.Core.Domain;

    public abstract class Specification<T> : ISpecification<T> where T : Entity
    {
        public abstract Expression<Func<T, bool>> ToExpression();
    }
}
