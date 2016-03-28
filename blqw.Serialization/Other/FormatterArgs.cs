using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    /// <summary>
    /// 序列化和反序列化时的额外参数
    /// </summary>
    public sealed class FormatterArgs: IFormatter
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
        
        object IFormatter.Deserialize(Stream serializationStream)
        {
            throw new NotImplementedException("不提供此功能");
        }

        void IFormatter.Serialize(Stream serializationStream, object graph)
        {
            throw new NotImplementedException("不提供此功能");
        }
    }
}
