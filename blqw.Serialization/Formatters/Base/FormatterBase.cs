using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供对象序列化和反序列化的格式化器抽象基类
    /// </summary>
    public abstract class FormatterBase : IFormatter
    {
        /// <summary>
        /// 获取或设置在反序列化过程中执行类型查找的 <see cref="SerializationBinder"/>
        /// </summary>
        public SerializationBinder Binder { get; set; }

        /// <summary>
        /// 获取或设置用于序列化和反序列化的 <see cref="StreamingContext"/>
        /// </summary>
        public StreamingContext Context { get; set; }

        /// <summary>
        /// 获取或设置当前格式化程序所使用的 <see cref="ISurrogateSelector"/>
        /// </summary>
        /// <remarks>周子鉴 2016.03.28</remarks>
        public ISurrogateSelector SurrogateSelector { get; set; }

        /// <summary>
        /// 反序列化所提供流中的数据并重新组成对象图形。
        /// </summary>
        /// <param name="serializationStream">包含要反序列化的数据的流。</param>
        /// <returns></returns>
        public abstract object Deserialize(Stream serializationStream);
        /// <summary>
        /// 将对象或具有给定根的对象图形序列化为所提供的流。
        /// </summary>
        /// <param name="serializationStream">格式化程序在其中放置序列化数据的流。 此流可以引用多种后备存储区（如文件、网络、内存等）。</param>
        /// <param name="graph">要序列化的对象或对象图形的根。 将自动序列化此根对象的所有子对象。</param>
        public abstract void Serialize(Stream serializationStream, object graph);




        protected static readonly TypeBinder DefaultBinder = new TypeBinder();

        /// <summary>
        /// 序列化 <see cref="Type"/> 对象
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="serializationStream"></param>
        /// <param name="type"></param>
        protected void SerializeType(Stream serializationStream, Type type)
        {
            var binder = Binder ?? DefaultBinder;
            string a, b;
            binder.BindToName(type, out a, out b);
            var stringFormatter = FormatterCache.GetStringFormatter(this);
            stringFormatter.Serialize(serializationStream, a);
            stringFormatter.Serialize(serializationStream, b);
        }

        protected Type DeserializeType(Stream serializationStream)
        {
            var binder = Binder ?? DefaultBinder;
            TraceDeserialize.WriteName("typeName");
            var stringFormatter = FormatterCache.GetStringFormatter(this);
            var a = stringFormatter.Deserialize(serializationStream);
            var b = stringFormatter.Deserialize(serializationStream);
            TraceDeserialize.WriteValue($"{a}, {b}");
            return binder.BindToType((string)a, (string)b);
        }
    }
}
