namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets.Queries
{
    using System;

    public abstract class Query<T> : IQuery<T>
    {
        private T result;
        protected Func<T> query;

        public virtual T Run()
        {
            if (query == null)
            {
                throw new InvalidOperationException("Cannot execute query because no query was set");
            }

            if (result == null || result.Equals(default(T)))
            {
                result = query();
            }

            return result;
        }
    }
}
