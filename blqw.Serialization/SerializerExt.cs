using blqw.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    static partial class Serializer
    {
        static readonly TypeBinder DefaultBinder = new TypeBinder();

        public static void SerializeType(this SerializationBinder binder, Stream serializationStream, Type type)
        {
            if (binder == null)
            {
                binder = CurrentBinder ?? DefaultBinder;
            }
            string a, b;
            binder.BindToName(type, out a, out b);
            FormatterCache.StringFormatter.Serialize(serializationStream, a);
            FormatterCache.StringFormatter.Serialize(serializationStream, b);
        }

        public static Type DeserializeType(this SerializationBinder binder, Stream serializationStream)
        {
            TraceDeserialize.WriteName("typeName");
            if (binder == null)
            {
                binder = CurrentBinder ?? DefaultBinder;
            }
            var a = FormatterCache.StringFormatter.Deserialize(serializationStream);
            var b = FormatterCache.StringFormatter.Deserialize(serializationStream);
            TraceDeserialize.WriteValue($"{a}, {b}");
            return binder.BindToType((string)a, (string)b);
        }

        public static T GetContextValue<T>(this StreamingContext context, T defaultValue)
        {
            if (context.Context != null && context.Context is T)
            {
                return (T)context.Context;
            }
            if (CurrentContext.Context != null && CurrentContext.Context is T)
            {
                return (T)CurrentContext.Context;
            }
            return default(T);
        }
        
    }
}
