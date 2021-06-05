namespace ExtraOptions
{
    using System;
    using UnityEngine;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif

    public class Vector3Converter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Vector3)value;
            serializer.Serialize(writer, new [] { c.x, c.y, c.z });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var v = (float[])serializer.Deserialize(reader, typeof(float[]));
            return new Vector3(v[0], v[1], v[2]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }
}