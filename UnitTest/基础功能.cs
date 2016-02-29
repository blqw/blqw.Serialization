using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace blqw.Serializable
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void 测试实体对象()
        {
            var a = new User{ id = 1,name = "blqw" };
            var bytes = Serializer.GetBytes(a);
            var b = (User)Serializer.GetObject(bytes);
            Assert.IsNotNull(b);
            Assert.AreEqual(a.id, b.id);
            Assert.AreEqual(a.name, b.name);


            var str = Serializer.GetString(a);
            var c = (User)Serializer.GetObject(str);
            Assert.IsNotNull(c);
            Assert.AreEqual(a.id, c.id);
            Assert.AreEqual(a.name, c.name);
        }
    }
}
