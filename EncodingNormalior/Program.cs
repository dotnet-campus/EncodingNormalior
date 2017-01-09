using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;

namespace EncodingNormalior
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //测试
            //ReadByte();
            
            string folder = "E:\\程序\\公司\\EncodingNormalior\\EncodingNormalior\\textFile";
            EncodingScrutator encodingScrutator=new EncodingScrutator();
            foreach (var temp in Directory.GetFiles(folder))
            {
                var file = new FileInfo(temp);
                Encoding encoding = encodingScrutator.InspectFileEncoding(file);
                Console.WriteLine(file.Name+" "+encoding.EncodingName+"\r\n");
            }
        }

        //private static void ReadByte()
        //{
        //    string file = "E:\\程序\\公司\\EncodingNormalior\\EncodingNormalior\\textFile\\GBK 3.txt";
        //    Stream stream=new FileStream(file,FileMode.Open);
        //    int n =(int) stream.Length;
        //    byte[] buffer=new byte[n];
        //    stream.Read(buffer, 0, n);
        //    //aa 文206 196
        //    //aa文三 97 97 206 196 200 253
        //}
    }


}
