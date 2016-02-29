using System;
using System.Collections.Generic;
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
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class TypeValueFormatter : ObjectFormatter
    {
        private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.ValueType;
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var typeName = (string)FormatterCache.StringFormatter.Deserialize(serializationStream);
            var type = Type.GetType(typeName, false);
            if (type == null)
            {
                //TODO:以后处理
                throw new SerializationException($"没有找到[{typeName}]类型");
            }
            var reftype = typeof(TypeValueReference<>).MakeGenericType(type);
            var obj = (IServiceProvider)FormatterServices.GetUninitializedObject(reftype);
            var refobj = TypedReference.MakeTypedReference(obj, reftype.GetFields());
            while (!type.Equals(typeof(object)))
            {
                var count = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
                var fields = type.GetFields(FLAGS);
                for (int i = 0; i < count; i++)
                {
                    var name = (string)FormatterCache.StringFormatter.Deserialize(serializationStream);
                    var value = Serializer.Read(serializationStream);
                    var field = GetField(fields, i, name);
                    if (field != null)
                    {
                        field.SetValueDirect(refobj, value);
                    }
                }
                type = type.BaseType;
            }

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
