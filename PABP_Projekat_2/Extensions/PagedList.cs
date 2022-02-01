using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PABP_Projekat_2.Extensions
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; }
        public int LastPage { get; }
        public PagedList(List<T> items, int currPage, int lastPage)
        {
            AddRange(items);
            CurrentPage = currPage;
            LastPage = lastPage;
        }  
    }
}
