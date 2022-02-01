using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PABP_Projekat_2.Extensions
{
    public static class PagingExtension
    {
        private static IQueryable<T> Paging<T>(this IQueryable<T> list, int pageIndex, int pageSize)
        {
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        private static IEnumerable<T> Paging<T>(this IEnumerable<T> list, int pageIndex, int pageSize)
        {
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> items, int pageIndex, int pageSize)
        {
            int count = await items.CountAsync();
            if (pageIndex < 1) pageIndex = 1;
            int lastPage = (int)Math.Ceiling(count / (decimal)pageSize);
            if (pageIndex > lastPage) pageIndex = lastPage;
            return new PagedList<T>(await items.Paging(pageIndex, pageSize).ToListAsync(), pageIndex, lastPage);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> items, int pageIndex, int pageSize)
        {
            int count = items.Count();
            if (pageIndex < 1) pageIndex = 1;
            int lastPage = (int)Math.Ceiling(count / (decimal)pageSize);
            if (pageIndex > lastPage) pageIndex = lastPage;
            return new PagedList<T>(items.Paging(pageIndex, pageSize).ToList(), pageIndex, lastPage);
        }

        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> items, int pageIndex, int pageSize)
        {
            int count = items.Count();
            if (pageIndex < 1) pageIndex = 1;
            int lastPage = (int)Math.Ceiling(count / (decimal)pageSize);
            if (pageIndex > lastPage) pageIndex = lastPage;
            return new PagedList<T>(items.Paging(pageIndex, pageSize).ToList(), pageIndex, lastPage);
        }
    }
}
