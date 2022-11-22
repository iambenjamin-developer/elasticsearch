using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Commons
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }


        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public string ToJson()
        {
            var res = new
            {
                CurrentPage,
                TotalPages,
                PageSize,
                TotalCount,
                HasPrevious,
                HasNext,
                Items = this
            };

            return JsonConvert.SerializeObject(res);
            //return JsonSerializer.Serialize(res);
        }
    }
#nullable enable
    public class PagedList<T, K> : List<K> where T : class where K : class?
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<K> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }


        public static PagedList<T, K> ToPagedList(PagedList<T> source, IMapper _mapper,
            Action<IMappingOperationOptions<List<T>, List<K>>>? opts = null)
        {
            var items = source.ToList();
            if (opts != null)
            {
                return new PagedList<T, K>(_mapper.Map(items, opts), source.TotalCount, source.CurrentPage,
                    source.PageSize);
            }
            else
            {
                return new PagedList<T, K>(_mapper.Map<List<T>, List<K>>(items), source.TotalCount,
                    source.CurrentPage, source.PageSize);
            }
        }

        public static PagedList<T, K> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize,
            IMapper _mapper, Action<IMappingOperationOptions<T, K>> opts)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T, K>(_mapper.Map<List<T>, List<K>>(items), count, pageNumber, pageSize);
        }

        public string ToJson()
        {
            var res = new
            {
                CurrentPage,
                TotalPages,
                PageSize,
                TotalCount,
                HasPrevious,
                HasNext,
                Items = this
            };

            return JsonConvert.SerializeObject(res);
            //return JsonSerializer.Serialize(res);
        }
    }
}
