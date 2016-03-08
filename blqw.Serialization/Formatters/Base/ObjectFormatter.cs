using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    /// <summary>
    /// 提供对象序列化和反序列化抽象基类
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public class ObjectFormatter : IFormatter
    {
        private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        /// <summary>
        /// 获取或设置在反序列化过程中执行类型查找的 <see cref="SerializationBinder"/>
        /// ,该字段在当前版本中无法设置
        /// </summary>
        public SerializationBinder Binder
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取或设置用于序列化和反序列化的 <see cref="StreamingContext"/>,该字段在当前版本中无法设置
        /// </summary>
        public StreamingContext Context
        {
            get
            {
                return new StreamingContext(StreamingContextStates.All, null);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取或设置当前格式化程序所使用的 <see cref="SurrogateSelector"/> ,该字段在当前版本中无法设置
        /// </summary>
        public ISurrogateSelector SurrogateSelector
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 绑定格式化类型
        /// </summary>
        public virtual Type BindType { get { return null; } }

        /// <summary>
        /// 格式化片段类型
        /// </summary>
        public virtual FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.Object;
            }
        }
        
        /// <summary>
        /// 反序列化所提供流中的数据并重新组成对象图形
        /// </summary>
        /// <param name="serializationStream">包含要反序列化的数据的流</param>
        /// <returns></returns>
        public virtual object Deserialize(Stream serializationStream)
        {
            TraceDeserialize.Write("(");
            if (serializationStream.ReadByte() == 0)
            {
                TraceDeserialize.WriteValue(null);
                return null;
            }
            TraceDeserialize.WriteName("typeName");
            var typeName = (string)FormatterCache.StringFormatter.Deserialize(serializationStream);
            var type = Type.GetType(typeName, false);
            if (type == null)
            {
                //TODO:以后处理
                throw new SerializationException($"没有找到[{typeName}]类型");
            }
            var obj = FormatterServices.GetUninitializedObject(type);//跳过构造函数创建对象
            ReferencedCache.Add(obj);
            while (!type.Equals(typeof(object)))
            {
                TraceDeserialize.WriteName($"{type.Name}.fieldCount");
                var count = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
                for (int i = 0; i < count; i++)
                {
                    TraceDeserialize.WriteName("fieldName");
                    var name = (string)FormatterCache.StringFormatter.Deserialize(serializationStream);
                    var value = Serializer.Read(serializationStream);

                    var field = type.GetField(name, FLAGS);
                    if (field != null)
                    {
                        var handler = FieldCache.Get(field);
                        handler.SetValue(obj, value);
                    }
                }
                type = type.BaseType;
            }

            TraceDeserialize.Write(")");
            return obj;
        }

        /// <summary>
        /// 将对象或具有给定根的对象图形序列化为所提供的流
        /// </summary>
        /// <param name="serializationStream">格式化程序在其中放置序列化数据的流</param>
        /// <param name="graph">要序列化的对象</param>
        public virtual void Serialize(Stream serializationStream, object graph)
        {
            if (graph == null)
            {
                serializationStream.WriteByte(0); //表示null
                return;
            }
            serializationStream.WriteByte(1); //表示有值

            var type = graph.GetType();
            FormatterCache.StringFormatter.Serialize(serializationStream, type.AssemblyQualifiedName);

            while (!type.Equals(typeof(object)))
            {
                var fields = FieldCache.GetByType(type);
                FormatterCache.Int32Formatter.Serialize(serializationStream, fields.Length);
                foreach (var f in fields)
                {
                    FormatterCache.StringFormatter.Serialize(serializationStream, f.Name);
                    Serializer.Write(serializationStream, f.GetValue(graph));
                }
                type = type.BaseType;
            }
        }
    }
}
