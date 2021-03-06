﻿using System;

using Cassandra;

namespace SkbKontur.DbViewer.Cql.Utils.ObjectsParser.ParseHelpers
{
    internal static class TimeUuidParseHelper
    {
        public static bool TryParse(string value, out TimeUuid result)
        {
            result = default;
            if (!Guid.TryParse(value, out var guid))
                return false;
            result = guid;
            return true;
        }
    }
}