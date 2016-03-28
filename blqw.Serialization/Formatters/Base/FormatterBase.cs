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
        /// 获取或设置当前格式化程序所使用的 <see cref="SurrogateSelector"/>
        /// </summary>
        /// <remarks>周子鉴 2016.03.28</remarks>
        [Obsolete("该字段在当前版本中无法设置")]
        public ISurrogateSelector SurrogateSelector { get; set; }

        public abstract object Deserialize(Stream serializationStream);
        public abstract void Serialize(Stream serializationStream, object graph);
    }
}
