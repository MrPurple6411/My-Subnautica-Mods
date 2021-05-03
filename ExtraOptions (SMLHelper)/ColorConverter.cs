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
            Color c = (Color)value;
            serializer.Serialize(writer, new float[3] { c.r, c.g, c.b });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            float[] v = (float[])serializer.Deserialize(reader, typeof(float[]));
            return new Color(v[0], v[1], v[2]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}