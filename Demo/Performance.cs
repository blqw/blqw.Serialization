using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    static class Performance
    {

        public static void Run()
        {
            _buffer = new byte[4];

            CodeTimer.Initialize();

            int count = 10000000;
            MemoryStream stream = new MemoryStream();

            CodeTimer.Time("位移 序列化", count, () => Serialize1(stream, count));
            CodeTimer.Time("指针 序列化", count, () => Serialize2(stream, count));
            CodeTimer.Time("bit 序列化", count, () => Serialize3(stream, count));

            CodeTimer.Time("位移 反序列化", count, () => Deserialize1(stream));
            CodeTimer.Time("指针 反序列化", count, () => Deserialize2(stream));
            CodeTimer.Time("bit 反序列化", count, () => Deserialize3(stream));

            Console.WriteLine("完成");
        }


        public static int Deserialize1(Stream serializationStream)
        {
            serializationStream.Position = 0;
            var a = (byte)serializationStream.ReadByte();
            var b = (byte)serializationStream.ReadByte();
            var c = (byte)serializationStream.ReadByte();
            var d = (byte)serializationStream.ReadByte();
            return a | (b << 8) | (c << 16) | (d << 24);
        }

        public static void Serialize1(Stream serializationStream, int graph)
        {
            serializationStream.Position = 0;
            var i = (int)graph;
            var a = (byte)((i) & 0xFF);
            var b = (byte)((i >> 8) & 0xFF);
            var c = (byte)((i >> 16) & 0xFF);
            var d = (byte)((i >> 24) & 0xFF);
            serializationStream.WriteByte(a);
            serializationStream.WriteByte(b);
            serializationStream.WriteByte(c);
            serializationStream.WriteByte(d);
        }


        public static int Deserialize2(Stream serializationStream)
        {
            serializationStream.Position = 0;
            var i = 0;
            unsafe
            {
                var p = (byte*)&i;
                p[0] = (byte)serializationStream.ReadByte();
                p[1] = (byte)serializationStream.ReadByte();
                p[2] = (byte)serializationStream.ReadByte();
                p[3] = (byte)serializationStream.ReadByte();
            }
            return i;
        }

        public static void Serialize2(Stream serializationStream, int graph)
        {
            serializationStream.Position = 0;
            var i = (int)graph;
            unsafe
            {
                var p = (byte*)&i;
                serializationStream.WriteByte(p[0]);
                serializationStream.WriteByte(p[1]);
                serializationStream.WriteByte(p[2]);
                serializationStream.WriteByte(p[3]);
            }
        }

        [ThreadStatic]
        static byte[] _buffer = new byte[4];

        public static int Deserialize3(Stream serializationStream)
        {
            serializationStream.Position = 0;
            if (_buffer == null)
            {
                _buffer = new byte[4];
            }
            serializationStream.Read(_buffer, 0, 4);
            return BitConverter.ToInt32(_buffer, 0);
        }

        public static void Serialize3(Stream serializationStream, int graph)
        {
            serializationStream.Position = 0;
            var bs = BitConverter.GetBytes(graph);
            serializationStream.WriteByte(bs[0]);
            serializationStream.WriteByte(bs[1]);
            serializationStream.WriteByte(bs[2]);
            serializationStream.WriteByte(bs[3]);
        }

    }
}
