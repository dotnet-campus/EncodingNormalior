using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncodingNormalizer.UnitTest
{
    [TestClass]
    public class ArgTest
    {
        [TestMethod]
        public void NoFolderArgTest()
        {
            //没有存在文件夹
            string[] arg = new[]
            {
                "-f",
                //"E:\\程序\\ethylene156\\EncodingNormalior"
            };

            try
            {
                EncodingNormalior.Program.Main(arg);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(ArgumentException));
                Assert.AreEqual(e.Message, EncodingNormalior.Program.NoFolderArgException);
            }
        }
    }
}