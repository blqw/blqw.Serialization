using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="char"/>对象的序列化和反序列化
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class CharFormatter:ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.Char;
            }
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var i = (char)graph;
            unsafe
            {
                var p = (byte*)&i;
                serializationStream.WriteByte(p[0]);
                serializationStream.WriteByte(p[1]);
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var i = new char();
            unsafe
            {
                var p = (byte*)&i;
                p[0] = (byte)serializationStream.ReadByte();
                p[1] = (byte)serializationStream.ReadByte();
            }
            TraceDeserialize.WriteValue(i);
            return i;
        }
    }
}
