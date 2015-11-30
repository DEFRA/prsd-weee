//The MIT license

//Copyright (c) 2008-2014 Martijn Boland, Bart Lenaerts, Rajeesh CV

//Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the
//"Software"), to deal in the Software without restriction, including
//without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to
//permit persons to whom the Software is furnished to do so, subject to
//the following conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
namespace EA.Weee.Core.Shared.Paging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(IEnumerable<T> source, int index, int pageSize, int? totalCount = null)
            : this(source.AsQueryable(), index, pageSize, totalCount)
        {
        }

        public PagedList(IQueryable<T> source, int index, int pageSize, int? totalCount = null)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Value can not be below 0.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", "Value can not be less than 1.");
            }

            if (source == null)
            {
                source = new List<T>().AsQueryable();
            }

            var realTotalCount = source.Count();

            PageSize = pageSize;
            PageIndex = index;
            TotalItemCount = totalCount ?? realTotalCount;
            PageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;

            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < (PageCount - 1));
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= (PageCount - 1));

            ItemStart = (PageIndex * PageSize) + 1;
            ItemEnd = Math.Min((PageIndex * PageSize) + PageSize, TotalItemCount);

            if (TotalItemCount <= 0)
            {
                return;
            }

            var realTotalPages = (int)Math.Ceiling(realTotalCount / (double)PageSize);

            if (realTotalCount < TotalItemCount && realTotalPages <= PageIndex)
            {
                AddRange(source.Skip((realTotalPages - 1) * PageSize).Take(PageSize));
            }
            else
            {
                AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
            }
        }
        
        public PagedList()
        {
        }

        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageNumber 
        {
            get
            {
                return PageIndex + 1;
            } 
        }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
        public int ItemStart { get; private set; }
        public int ItemEnd { get; private set; }
    }
}