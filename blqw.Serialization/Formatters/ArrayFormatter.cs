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
    /// 提供数组对象的序列化和反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class ArrayFormatter : ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.Array;
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var typeName = (string)FormatterCache.StringFormatter.Deserialize(serializationStream);
            var type = Type.GetType(typeName, false);
            if (type == null)
            {
                throw new SerializationException($"数组类型[{typeName}]错误");
            }

            Func<Stream, object> deserialize;
            if (type.IsValueType || type.IsSealed) //值类型或者密封类,不会有子类
            {
                deserialize = FormatterCache.GetFormatter(type).Deserialize;
            }
            else
            {
                deserialize = Serializer.Read;
            }
            var rank = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
            Array array;
            //一维数组
            if (rank == 1)
            {
                var length = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
                array = Array.CreateInstance(type, length);
                ReferencedCache.Add(array);
                for (int i = 0; i < length; i++)
                {
                    array.SetValue(deserialize(serializationStream), i);
                }
                return array;
            }
            int[] indexes = new int[rank];
            //多维数组
            for (int i = 0; i < rank; i++)
            {
                indexes[i] = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
            }
            array = Array.CreateInstance(type, indexes);
            Deserialize(serializationStream, array, 0, array.Rank-1, indexes, deserialize);
            return array;
        }

        /// <summary>
        /// 多位数组递归赋值
        /// </summary>
        /// <param name="serializationStream">反序列化数组流</param>
        /// <param name="array">数组</param>
        /// <param name="currentRank">当前纬度</param>
        /// <param name="maxRank">最大纬度</param>
        /// <param name="indexes">索引</param>
        /// <param name="deserialize">用于反序列化的委托</param>
        private void Deserialize(Stream serializationStream, Array array, int currentRank, int maxRank, int[] indexes, Func<Stream, object> deserialize)
        {
            var upperBound = array.GetUpperBound(currentRank);
            if (currentRank == maxRank)
            {
                for (int i = 0; i <= upperBound; i++)
                {
                    indexes[currentRank] = i;
                    array.SetValue(deserialize(serializationStream), indexes);
                }
                return;
            }
            for (int i = 0; i <= upperBound; i++)
            {
                indexes[currentRank] = i;
                Deserialize(serializationStream, array, currentRank + 1, maxRank, indexes, deserialize);
            }
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var array = (Array)graph;
            var type = graph.GetType().GetElementType();
            FormatterCache.StringFormatter.Serialize(serializationStream, type.AssemblyQualifiedName);

            Action<Stream, object> serialize;
            if (type.IsValueType || type.IsSealed) //值类型或者密封类,不会有子类
            {
                serialize = FormatterCache.GetFormatter(type).Serialize;
            }
            else
            {
                serialize = Serializer.Write;
            }
            
            FormatterCache.Int32Formatter.Serialize(serializationStream, array.Rank);
            if (array.Rank == 1)
            {
                var length = array.Length;
                FormatterCache.Int32Formatter.Serialize(serializationStream, length);
                for (int i = 0; i < length; i++)
                {
                    serialize(serializationStream, array.GetValue(i));
                }
                return;
            }

            //多维数组
            for (int i = 0; i < array.Rank; i++)
            {
                FormatterCache.Int32Formatter.Serialize(serializationStream, array.GetUpperBound(i));
            }
            int[] indexes = new int[array.Rank];
            Serialize(serializationStream, array, 0, array.Rank-1, indexes, serialize);
        }

        /// <summary>
        /// 多维数组递归写入流
        /// </summary>
        /// <param name="serializationStream">需要写入数据的流</param>
        /// <param name="array">数组</param>
        /// <param name="currentRank">当前纬度</param>
        /// <param name="maxRank">最大纬度</param>
        /// <param name="indexes">索引</param>
        /// <param name="serialize">用于序列化的委托</param>
        private void Serialize(Stream serializationStream, Array array, int currentRank, int maxRank, int[] indexes, Action<Stream, object> serialize)
        {
            var upperBound = array.GetUpperBound(currentRank);
            if (currentRank == maxRank)
            {
                for (int i = 0; i <= upperBound; i++)
                {
                    indexes[currentRank] = i;
                    serialize(serializationStream, array.GetValue(indexes));
                }
                return;
            }
            for (int i = 0; i <= upperBound; i++)
            {
                indexes[currentRank] = i;
                Serialize(serializationStream, array, currentRank + 1, maxRank, indexes, serialize);
            }
        }
    }
}
