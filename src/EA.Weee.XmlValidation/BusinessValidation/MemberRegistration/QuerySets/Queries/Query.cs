namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries
{
    using System;

    public abstract class Query<T> : IQuery<T>
    {
        private T result;
        protected Func<T> query;
        private bool hasRun;

        protected Query()
        {
            hasRun = false;
        }

        public virtual T Run()
        {
            if (query == null)
            {
                throw new InvalidOperationException("Cannot execute query because no query was set");
            }

            if (!hasRun)
            {
                result = query();
                hasRun = true;
            }

            return result;
        }
    }
}
