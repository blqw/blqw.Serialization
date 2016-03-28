using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 值类型对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(ValueType))]
    [ExportMetadata("HeadFlag", HeadFlag.ValueType)]
    public sealed class TypeValueFormatter : ObjectFormatter
    {
        private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        
        public override object Deserialize(Stream serializationStream)
        {
            serializationStream.ReadByte(); //值类型不存在null 所以第一个字节忽略
            TraceDeserialize.Write("(");
            var type = DeserializeType(serializationStream);
            if (type == null)
            {
                throw new SerializationException($"反序列化时出现错误 SerializationBinder 返回为null");
            }
            var reftype = typeof(TypeValueReference<>).MakeGenericType(type);
            var obj = (IServiceProvider)FormatterServices.GetUninitializedObject(reftype);
            var refobj = TypedReference.MakeTypedReference(obj, reftype.GetFields());
            while (!type.Equals(typeof(object)))
            {
                TraceDeserialize.WriteName("fieldCount");
                var count = (int)FormatterCache.GetInt32Formatter(this).Deserialize(serializationStream);
                var fields = type.GetFields(FLAGS);
                for (int i = 0; i < count; i++)
                {
                    TraceDeserialize.WriteName("fieldName");
                    var name = (string)FormatterCache.GetStringFormatter(this).Deserialize(serializationStream);
                    var value = Serializer.Read(serializationStream, this);

                    var field = GetField(fields, i, name);
                    if (field != null)
                    {
                        field.SetValueDirect(refobj, value);
                    }
                    else
                    {
                        Serializer.Read(serializationStream, this);
                    }
                }
                type = type.BaseType;
            }

            TraceDeserialize.Write(")");
            return obj.GetService(null);
        }

        class TypeValueReference<T> : IServiceProvider
        {
            public T Value;

            public object GetService(Type serviceType)
            {
                return Value;
            }
        }

        private static FieldInfo GetField(FieldInfo[] fields, int index, string name)
        {
            if (index < fields.Length)
            {
                var f = fields[index];
                if (f.Name == name)
                {
                    return f;
                }
            }

            for (int i = 0; i < fields.Length; i++)
            {
                var f = fields[i];
                if (f.Name == name)
                {
                    return f;
                }
            }

            return null;
        }
    }
}
