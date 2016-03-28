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
    /// 提供任意对象序列化和反序列化
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(object))]
    [ExportMetadata("HeadFlag", HeadFlag.Object)]
    public class ObjectFormatter : FormatterBase
    {
        private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        static readonly Type ObejctType = typeof(object);
        /// <summary>
        /// 反序列化所提供流中的数据并重新组成对象图形
        /// </summary>
        /// <param name="serializationStream">包含要反序列化的数据的流</param>
        /// <returns></returns>
        public override object Deserialize(Stream serializationStream)
        {
            if (serializationStream.ReadByte() == 0)
            {
                return null;
            }
            var type = DeserializeType(serializationStream);
            if (type == null)
            {
                throw new SerializationException($"反序列化时出现错误 SerializationBinder 返回为null");
            }
            
            var obj = FormatterServices.GetUninitializedObject(type);//跳过构造函数创建对象
            ReferencedCache.Add(obj);
            var stringFormatter = FormatterCache.GetStringFormatter(this);
            var intFormatter = FormatterCache.GetInt32Formatter(this);
            while (!ObejctType.Equals(type ?? ObejctType))
            {
                var count = (int)intFormatter.Deserialize(serializationStream);
                for (int i = 0; i < count; i++)
                {
                    var name = (string)stringFormatter.Deserialize(serializationStream);
                    var value = Serializer.Read(serializationStream,this);

                    var field = type.GetField(name, FLAGS);
                    if (field != null)
                    {
                        var handler = FieldCache.Get(field);
                        handler.SetValue(obj, value);
                    }
                }
                type = type.BaseType;
            }
            return obj;
        }

        /// <summary>
        /// 将对象或具有给定根的对象图形序列化为所提供的流
        /// </summary>
        /// <param name="serializationStream">格式化程序在其中放置序列化数据的流</param>
        /// <param name="graph">要序列化的对象</param>
        public override void Serialize(Stream serializationStream, object graph)
        {
            if (graph == null)
            {
                serializationStream.WriteByte(0); //表示null
                return;
            }
            serializationStream.WriteByte(1); //表示有值

            var type = graph.GetType();
            SerializeType(serializationStream, type);

            var stringFormatter = FormatterCache.GetStringFormatter(this);
            var intFormatter = FormatterCache.GetInt32Formatter(this);
            while (!ObejctType.Equals(type ?? ObejctType))
            {
                var fields = FieldCache.GetByType(type);
                intFormatter.Serialize(serializationStream, fields.Length);
                foreach (var f in fields)
                {
                    stringFormatter.Serialize(serializationStream, f.Name);
                    Serializer.Write(serializationStream, f.GetValue(graph),this);
                }
                type = type.BaseType;
            }
        }
    }
}
