using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw.SerializationComponent;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="SerializationInfo"/> 类型的序列化和反序列化方法
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(SerializationInfo))]
    [ExportMetadata("HeadFlag", HeadFlag.SerializationInfo)]
    public sealed class SerializationInfoFormatter : FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            if (SurrogateSelector == null)
            {
                throw new ArgumentNullException(nameof(SurrogateSelector), "反序列化时出现错误: 代理项选择器 不存在");
            }

            var binder = Binder ?? DefaultBinder;
            var stringFormatter = FormatterCache.GetStringFormatter(this);
            var a = stringFormatter.Deserialize(serializationStream);
            var b = stringFormatter.Deserialize(serializationStream);
            var type = binder.BindToType((string)a, (string)b);
            if (type == null)
            {
                var ex = new SerializationException($"反序列化时出现错误: SerializationBinder.BindToType 返回为null");
                ex.Data.Add("assemblyName", (string)a);
                ex.Data.Add("typeName", (string)b);
                throw ex;
            }
            ISurrogateSelector selector;
            var surrogate = SurrogateSelector.GetSurrogate(type, Context, out selector);
            if (surrogate == null)
            {
                throw new ArgumentNullException(nameof(surrogate), "反序列化时出现错误: 无法获取代理项");
            }
            var c = FormatterCache.GetInt32Formatter(this).Deserialize(serializationStream);
            var info = new SerializationInfo(type, Component.Converter);
            for (int i = 0, length = (int)c; i < length; i++)
            {
                a = stringFormatter.Deserialize(serializationStream);
                b = stringFormatter.Deserialize(serializationStream);
                type = binder.BindToType((string)a, (string)b);
                if (type == null)
                {
                    var ex = new SerializationException($"反序列化时出现错误: SerializationBinder.BindToType 返回为null");
                    ex.Data.Add("assemblyName", (string)a);
                    ex.Data.Add("typeName", (string)b);
                    throw ex;
                }
                var name = stringFormatter.Deserialize(serializationStream);
                var value = Serializer.Read(serializationStream, this);
                info.AddValue((string)name, value, type);
            }

            var obj = Activator.CreateInstance(type);
            return surrogate.SetObjectData(obj, info, Context, selector);
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var info = (SerializationInfo)graph;

            var stringFormatter = FormatterCache.GetStringFormatter(this);
            stringFormatter.Serialize(serializationStream, info.AssemblyName);
            stringFormatter.Serialize(serializationStream, info.FullTypeName);
            FormatterCache.GetInt32Formatter(this).Serialize(serializationStream, info.MemberCount);

            var binder = Binder ?? DefaultBinder;
            foreach (var item in info)
            {
                string a, b;
                binder.BindToName(item.ObjectType, out a, out b);
                stringFormatter.Serialize(serializationStream, a);
                stringFormatter.Serialize(serializationStream, b);
                stringFormatter.Serialize(serializationStream, item.Name);
                Serializer.Write(serializationStream, item.Value, this);
            }
        }
    }
}
