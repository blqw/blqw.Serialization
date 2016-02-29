﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="DBNull"/>对象的序列化和反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class DBNullFormatter:ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.DBNull;
            }
        }
        public override void Serialize(Stream serializationStream, object graph)
        {

        }
        public override object Deserialize(Stream serializationStream)
        {
            return DBNull.Value;
        }
    }
}
