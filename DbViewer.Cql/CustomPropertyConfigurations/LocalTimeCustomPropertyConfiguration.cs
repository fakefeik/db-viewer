﻿using System;
using System.Reflection;

using Cassandra;

using SkbKontur.DbViewer.TypeAndObjectBuilding;

namespace SkbKontur.DbViewer.Cql.CustomPropertyConfigurations
{
    public static class LocalTimeCustomPropertyConfiguration
    {
        public static CustomPropertyConfiguration TryGetConfiguration(PropertyInfo propertyInfo)
        {
            if (!(propertyInfo.PropertyType == typeof(LocalTime)))
                return null;

            return new CustomPropertyConfiguration
                {
                    ResolvedType = typeof(DateTime?),
                    StoredToApi = @object => ((LocalTime)@object).ToDateTime(),
                    ApiToStored = @object => ((DateTime?)@object).ToLocalTime(),
                };
        }
    }
}