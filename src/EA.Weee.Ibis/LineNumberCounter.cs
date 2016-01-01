namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Internal class used to help with tracking the numbers of lines written.
    /// The number of the first line will be 0.
    /// </summary>
    internal class LineNumberCounter
    {
        private int mCurrent = 0;

        /// <summary>
        /// Returns the number of the next line.
        /// </summary>
        public int Next
        {
            get
            {
                int result = mCurrent;
                mCurrent++;
                return result;
            }
        }
    }
}