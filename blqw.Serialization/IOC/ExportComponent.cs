using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw.Serialization;

namespace blqw.SerializationComponent
{

    static class ExportComponent
    {
        ///// <summary> 将数据序列化成字符串
        ///// </summary>
        [Export("SerializeToString")]
        [ExportMetadata("Priority", 100)]
        public static string SerializeToString(object input)
        {
            return null;// Serializer.GetString(input);
        }

        ///// <summary> 将数据序列化成字节流
        ///// </summary>
        [Export("SerializeToBytes")]
        [ExportMetadata("Priority", 100)]
        public static byte[] SerializeToBytes(object input)
        {
            return Serializer.GetBytes(input);
        }

        ///// <summary> 将字符串反序列化成对象
        ///// </summary>
        [Export("DeserializeFromString")]
        [ExportMetadata("Priority", 100)]
        public static object DeserializeFromString(string input)
        {
            return null;// Serializer.GetObject(input);
        }

        ///// <summary> 将字节流反序列化成对象
        ///// </summary>
        [Export("DeserializeFormBytes")]
        [ExportMetadata("Priority", 100)]
        public static object DeserializeFormBytes(byte[] input)
        {
            return Serializer.GetObject(input);
        }        
    }
}
