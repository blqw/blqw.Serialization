﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="DateTime"/>对象的序列化和反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class DateTimeFormatter:ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.DateTime;
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var i = new DateTime();
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
            }
            return i;
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var i = (DateTime)graph;
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
            }
        }
    }
}
