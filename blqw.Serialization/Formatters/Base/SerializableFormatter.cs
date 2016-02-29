﻿using System;
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
            if (_Formatter == null)
            {
                _Formatter = new BinaryFormatter();
            }
            return _Formatter.Deserialize(serializationStream);
        }

        [ThreadStatic]
        static BinaryFormatter _Formatter;

        public override void Serialize(Stream serializationStream, object graph)
        {
            if (_Formatter == null)
            {
                _Formatter = new BinaryFormatter();
            }
            _Formatter.Serialize(serializationStream, graph);
        }
    }
}
