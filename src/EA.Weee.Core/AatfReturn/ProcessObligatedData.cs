namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;

    public class ProcessObligatedData
    {
        public ProcessObligatedData(int processCategoryId, string processName, List<ObligatedData> weeeData, IList<ObligatedCategoryValue> categoryValues)
        {
            ProcessCategoryId = processCategoryId;
            ProcessName = processName;
            WeeeData = weeeData;
            CategoryValues = categoryValues;
        }
        
        public int ProcessCategoryId { get; set; }

        public string ProcessName { get; set; }

        public List<ObligatedData> WeeeData { get; set; }

        public IList<ObligatedCategoryValue> CategoryValues { get; set; }

        public ProcessObligatedData(ObligatedCategoryValues values)
        {
            AddCategoryValues(values);
        }

        private void AddCategoryValues(ObligatedCategoryValues obligatedCategories)
        {
            CategoryValues = new List<ObligatedCategoryValue>();

            foreach (var categoryValue in obligatedCategories)
            {
                CategoryValues.Add(categoryValue);
            }
        }
    }
}
