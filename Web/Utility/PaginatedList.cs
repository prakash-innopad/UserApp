using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Utility
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool PreviousPage

        {
            get
            {
                return (PageIndex > 0);
            }
        }
        public bool Nextpage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }

        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize, int count)
        {
           // var count = source.Count();
           // var items = source.Skip((pageIndex) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(source, count, pageIndex, pageSize);
        }
    }
}
