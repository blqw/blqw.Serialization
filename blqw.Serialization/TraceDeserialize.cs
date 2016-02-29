using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{

    public static class TraceDeserialize
    {
        public static void Reset()
        {
            _Buffer = null;
            _StopWirting = false;
        }

        [ThreadStatic]
        static StringBuilder _Buffer;
        [ThreadStatic]
        static bool _StopWirting;
        

        [Conditional("DEBUG")]
        public static void WriteValue(object value)
        {
            if (_StopWirting) return;
            if (_Buffer == null)
            {
                _Buffer = new StringBuilder();
            }
            _Buffer.Append(value?.ToString() ?? "<null>");
            _Buffer.Append(" | ");
        }

        [Conditional("DEBUG")]
        public static void WriteName(string name)
        {
            if (_StopWirting) return;
            if (_Buffer == null)
            {
                _Buffer = new StringBuilder();
            }
            _Buffer.Append($"<{name}>=");
        }

        [Conditional("DEBUG")]
        public static void WriteRef(int index)
        {
            if (_StopWirting) return;
            _Buffer.Append($"<<ref({index})>> | ");
        }


        public static string LastDebug()
        {
            return _Buffer?.ToString();
        }

        [Conditional("DEBUG")]
        public static void SetWriting(bool writing)
        {
            _StopWirting = !writing;
        }
    }
}
