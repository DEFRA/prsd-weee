﻿//The MIT license

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
namespace EA.Weee.Web.Infrastructure.Paging
{
    using System.Collections.Generic;

    public class PaginationModel
    {
        public int PageSize { get; internal set; }

        public int CurrentPage { get; internal set; }

        public int PageCount { get; internal set; }

        public int TotalItemCount { get; internal set; }

        public IList<PaginationLink> PaginationLinks { get; private set; }
        
        public PagerOptions Options { get; internal set; }

        public PaginationModel()
        {
            PaginationLinks = new List<PaginationLink>();
            Options = null;
        }
    }
}