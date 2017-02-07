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

    }
}
