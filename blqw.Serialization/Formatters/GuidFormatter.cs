﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="Guid"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(Guid))]
    [ExportMetadata("HeadFlag", HeadFlag.Guid)]
    public sealed class GuidFormatter:FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            var i = new Guid();
            unsafe
            {
                var p = (byte*)&i;
                p[0] = (byte)serializationStream.ReadByte();
                p[1] = (byte)serializationStream.ReadByte();
                p[2] = (byte)serializationStream.ReadByte();
                p[3] = (byte)serializationStream.ReadByte();
                p[4] = (byte)serializationStream.ReadByte();
                p[5] = (byte)serializationStream.ReadByte();
                p[6] = (byte)serializationStream.ReadByte();
                p[7] = (byte)serializationStream.ReadByte();
                p[8] = (byte)serializationStream.ReadByte();
                p[9] = (byte)serializationStream.ReadByte();
                p[10] = (byte)serializationStream.ReadByte();
                p[11] = (byte)serializationStream.ReadByte();
                p[12] = (byte)serializationStream.ReadByte();
                p[13] = (byte)serializationStream.ReadByte();
                p[14] = (byte)serializationStream.ReadByte();
                p[15] = (byte)serializationStream.ReadByte();
            }
            TraceDeserialize.WriteValue(i);
            return i;
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var i = (Guid)graph;
            unsafe
            {
                var p = (byte*)&i;
                serializationStream.WriteByte(p[0]);
                serializationStream.WriteByte(p[1]);
                serializationStream.WriteByte(p[2]);
                serializationStream.WriteByte(p[3]);
                serializationStream.WriteByte(p[4]);
                serializationStream.WriteByte(p[5]);
                serializationStream.WriteByte(p[6]);
                serializationStream.WriteByte(p[7]);
                serializationStream.WriteByte(p[8]);
                serializationStream.WriteByte(p[9]);
                serializationStream.WriteByte(p[10]);
                serializationStream.WriteByte(p[11]);
                serializationStream.WriteByte(p[12]);
                serializationStream.WriteByte(p[13]);
                serializationStream.WriteByte(p[14]);
                serializationStream.WriteByte(p[15]);
            }
        }
    }
}
