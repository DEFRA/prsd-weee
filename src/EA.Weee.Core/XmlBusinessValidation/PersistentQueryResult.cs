namespace EA.Weee.Core.XmlBusinessValidation
{
    using System;

    public class PersistentQueryResult<T>
    {
        private readonly Func<T> query;

        private T data;
        private bool hasBeenRetrieved;

        public PersistentQueryResult(Func<T> query)
        {
            hasBeenRetrieved = false;
            this.query = query;
        }

        public T Get()
        {
            if (!hasBeenRetrieved)
            {
                data = query();
                hasBeenRetrieved = true;
            }

            return data;
        }
    }
}
