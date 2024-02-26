using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Models
{
    public class PaginatedListData
    {
        public List<Profile> items { get; set; }
        public int DbCount { get; set; }
    }
}
