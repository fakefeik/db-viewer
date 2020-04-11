using Newtonsoft.Json;

namespace SkbKontur.DbViewer.VNext.DataTypes
{
    public class ObjectDescription : ObjectIdentifier
    {
        [NotNull]
        [JsonProperty("typeMetaInformation")]
        public TypeMetaInformation TypeMetaInformation { get; set; }
    }
}