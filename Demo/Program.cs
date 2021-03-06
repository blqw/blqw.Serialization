﻿using blqw.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    class Program
    {
        static void Main(string[] args)
        {
            object obj;
            try
            {
                obj = null;
                obj.ToString();
            }
            catch (Exception ex)
            {
                obj = ex;
            }
            try
            {
                var bytes = Serializer.GetBytes(obj);
                var my2 = Serializer.GetObject(bytes);


                var bytes3 = Serializer.GetString(obj);
                var my3 = Serializer.GetObject(bytes3);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
            finally
            {
                Console.WriteLine(TraceDeserialize.LastDebug());
            }

        }

    }

    class MyClass
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
