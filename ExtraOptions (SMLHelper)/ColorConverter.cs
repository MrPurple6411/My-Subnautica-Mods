namespace ExtraOptions
{
    using System;
    using UnityEngine;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif


    public class ColorConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Color)value;
            serializer.Serialize(writer, new[] { c.r, c.g, c.b });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var v = (float[])serializer.Deserialize(reader, typeof(float[]));
            return new Color(v[0], v[1], v[2]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}