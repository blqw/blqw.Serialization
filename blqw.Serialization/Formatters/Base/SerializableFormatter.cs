using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="ISerializable"/>对象的序列化和反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public abstract class SerializableFormatter : ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Type BindType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            if (serializationStream.ReadByte() == 0)
            {
                TraceDeserialize.WriteValue(null);
                return null;
            }
            if (_Formatter == null)
            {
                _Formatter = new BinaryFormatter();
            }
            TraceDeserialize.WriteValue(">>BinaryFormatter<<");
            return _Formatter.Deserialize(serializationStream);
        }

        [ThreadStatic]
        static BinaryFormatter _Formatter;

        public override void Serialize(Stream serializationStream, object graph)
        {
            if (graph == null)
            {
                serializationStream.WriteByte(0); //表示null
                return;
            }
            serializationStream.WriteByte(1); //表示有值

            if (_Formatter == null)
            {
                _Formatter = new BinaryFormatter();
            }
            _Formatter.Serialize(serializationStream, graph);
        }
    }
}
