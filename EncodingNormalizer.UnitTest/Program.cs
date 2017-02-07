using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncodingNormalizer.UnitTest
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
        }

        [TestMethod]
        public void MainTest()
        {
            string[] arg = new[]
              {
                "-f",
                "E:\\程序\\ethylene156\\EncodingNormalior"
            };

            EncodingNormalior.Program.Main(arg);
        }

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
            }
        }
    }
}
