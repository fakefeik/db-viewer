﻿using System.Threading.Tasks;

using SkbKontur.DbViewer.Connector;
using SkbKontur.DbViewer.Dto;
using SkbKontur.DbViewer.TestApi.Impl.Classes;

#pragma warning disable 1998

namespace SkbKontur.DbViewer.TestApi.Impl
{
    public class SampleDbConnector<T> : IDbConnector<T>
        where T : class
    {
        public async Task<object[]> Search(Filter[] filters, Sort[] sorts, int from, int count)
        {
            // ReSharper disable once CoVariantArrayConversion
            return SampleDataBase.Instance.Find(filters, sorts, from, count);
        }

        public async Task<int?> Count(Filter[] filters, int? limit)
        {
            return SampleDataBase.Instance.Count(filters, limit);
        }

        public async Task<object> Read(Filter[] filters)
        {
            return SampleDataBase.Instance.Read(filters);
        }

        public async Task Delete(object @object)
        {
            SampleDataBase.Instance.Delete((TestClass)@object);
        }

        public async Task<object> Write(object @object)
        {
            return SampleDataBase.Instance.Write((TestClass)@object);
        }
    }
}