using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }

        public PagedList(List<T> items , int pageNumber , int count, int pageSize )
        {
            PageNumber = pageNumber;
            TotalCount = count;
            PageSize = pageSize;
            TotalPage = (int)Math.Ceiling(count/(double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, Func<T,Boolean> func)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).Where<T>(u => func(u)).ToListAsync();
            return new PagedList<T>(items,count,pageNumber,pageSize);
        }
    }
}