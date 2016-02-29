﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供字符串对象序列化与反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class StringFormatter : ObjectFormatter
    {
        [ThreadStatic]
        static StringBuilder _Buffer;

        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.String;
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var length = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
            if (_Buffer == null)
            {
                _Buffer = new StringBuilder();
            }
            else
            {
                _Buffer.Clear();
            }

            for (int i = 0; i < length; i += 2)
            {
                var c = serializationStream.ReadByte()
                        | serializationStream.ReadByte() << 8;
                _Buffer.Append((char)c);
            }
            var str = _Buffer.ToString();
            //TODO:ReferencedCache.Add(str);
            return str;
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var str = (string)graph;
            var length = str.Length << 1; // ×2 ,一个char = 2个byte
            FormatterCache.Int32Formatter.Serialize(serializationStream, length);
            unsafe
            {
                fixed (char* p = str)
                {
                    var p1 = (byte*)p;
                    for (int i = 0; i < length; i++)
                    {
                        serializationStream.WriteByte(p1[i]);
                    }
                }
            }
        }
    }
}
