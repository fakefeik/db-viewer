﻿using Newtonsoft.Json;

namespace SkbKontur.DbViewer.Dto.TypeInfo
{
    public class DictionaryTypeInfo : TypeInfo
    {
        public DictionaryTypeInfo(TypeInfo key, TypeInfo value)
        {
            Key = key;
            Value = value;
        }

        [JsonProperty("type")]
        public override PrimitiveType Type => PrimitiveType.Dictionary;

        [JsonProperty("key")]
        public TypeInfo Key { get; }

        [JsonProperty("value")]
        public TypeInfo Value { get; }
    }
}