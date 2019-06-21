namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Prsd.Core.Domain;

    public abstract class Specification<T> : ISpecification<T> where T : Entity
    {
        public abstract Expression<Func<T, bool>> ToExpression();
    }
}
