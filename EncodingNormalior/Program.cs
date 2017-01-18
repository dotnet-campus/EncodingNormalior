using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EncodingNormalior.Model;

namespace EncodingNormalior
{
    public class Program
    {
        private const string IncludeFileCommand = "--IncludeFile";
        private const string WhiteListCommand = "--WhiteList";
        private const string EncodingCommand = "-E";
        private const string FolderCommand = "-f";
        private const string ProjectCommand = "-p";

        //        private const string Usage =
        //@"usage: EncodingNormalior <command> [args]
        //                         [--IncludeFile path] [--WhiteList path] [-E Encoding]
        //These are EncodingNormalior commands
        //  -f      folder
        //          the folder need to check all the file
        //          whether the file comply with Encoding

        // -p       csproj
        //          Check all the file whether the file 
        //          comply with Encoding in the project

        //编码规范工具使用方法：
        // 必选命令：输入要检测的文件夹或*csproj文件
        // 输入格式：
        //     输入文件夹      -f 文件夹路径
        //     输入csproj文件  -p csproj文件路径
        //     不能同时输入文件夹和csproj文件

        // 可选命令：
        // 输入格式：
        //    必须包含的文件    --IncludeFile 文件路径
        //    文件白名单       --WhiteList   文件路径
        //    规定的编码       -E            Encoding";

        private const string Usage =
@"usage: EncodingNormalior <command> [args]
                         [--IncludeFile path] [--WhiteList path] [-E Encoding]
These are EncodingNormalior commands
  -f      folder
          the folder need to check all the file
          whether the file comply with Encoding

编码规范工具使用方法：
 必选命令：输入要检测的文件夹或*csproj文件
 输入格式：
     输入文件夹      -f 文件夹路径

 可选命令：
 输入格式：
    必须包含的文件    --IncludeFile 文件路径   
    文件白名单       --WhiteList   文件路径
    规定的编码       -E            Encoding

                   
Encoding 包含 utf8、 gbk、 ascii、utf16、BigEndianUnicode
程序从输入的文件夹读取配置，越靠近文本的配置优先度越高。
如果不存在配置，使用默认配置。
白名单配置名称：        WhiteList.txt
必须包含的文件配置名称： IncludeFile.txt
要求配置编码是 UTF8";

        //目录高于全局
        //继承
        //输出不符合

        //有不符合Main返回不是0

        private static void ParseCommand(CommandLineArgumentParser arguments)
        {
            ConformCommand(arguments);
            var folder = arguments.Get(FolderCommand).Next;
            if (!Directory.Exists(folder))
            {
                throw new ArgumentException("不存在文件夹" + folder);
            }

            EncodingScrutatorFolder encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo(folder));
            if (arguments.Has(IncludeFileCommand))
            {
                var includeFile = arguments.Get(IncludeFileCommand).Take();
                var wildCardFile = IncludeFileSetting.ReadIncludeFile(includeFile);
                IncludeFileSetting includeFileSetting = new IncludeFileSetting(wildCardFile);
                encodingScrutatorFolder.IncludeFileSetting = includeFileSetting;
            }
            if (arguments.Has(EncodingCommand))
            {
                string encodingCommand = arguments.Get(EncodingCommand).Take();
                encodingCommand = encodingCommand.ToLower();
                Encoding encoding;
                switch (encodingCommand)
                {
                    case "utf8":
                    case "utf-8":
                        encoding = Encoding.UTF8;
                        break;
                    case "gbk":
                        encoding = Encoding.GetEncoding("gbk");
                        break;
                    case "bigendianunicode":
                        encoding = Encoding.BigEndianUnicode;
                        break;
                    default:
                        throw new ArgumentException("输入无法识别编码");
                }
                encodingScrutatorFolder.SitpulationEncodingSetting = new SitpulationEncodingSetting()
                {
                    SitpulationEncoding = encoding
                };
            }
            if (arguments.Has(WhiteListCommand))
            {
                var whiteListFile = arguments.Get(WhiteListCommand).Take();
                encodingScrutatorFolder.InspectFileWhiteListSetting = InspectFileWhiteListSetting.ReadWhiteListSetting(whiteListFile);
            }
            //EncodingScrutatorFolder encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo("E:\\程序\\公司\\EncodingNormalior"));

            //if()//检测文件编码
            encodingScrutatorFolder.InspectFolderEncoding();
            Console.WriteLine(PintnoConformEncodingFile(encodingScrutatorFolder));
            if (_illicitFile.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                str.Append("存在以下文件不符合规范\r\n");
                foreach (var temp in _illicitFile)
                {
                    //temp.File
                    str.Append(temp.File.FullName + "\r\n");
                }
                throw new Exception(str.ToString());
            }
        }



        private static void ConformCommand(CommandLineArgumentParser arguments)
        {
            if (!arguments.Has(FolderCommand))
            {
                throw new ArgumentException("需要包含要检测文件夹");
            }

            //if (!arguments.Has(ProjectCommand) && !arguments.Has(FolderCommand))
            //{
            //    throw new ArgumentException("需要包含要检测文件夹或csproj文件");
            //}
            //if (arguments.Has(ProjectCommand) && arguments.Has(FolderCommand))
            //{
            //    throw new ArgumentException("要检测文件夹或csproj文件只能包含一个");
            //}
        }

        public static void Main(string[] args)
        {
            try
            {
                var arguments = CommandLineArgumentParser.Parse(args);

                try
                {
                    ParseCommand(arguments);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Usage);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(-1);
            }
            Environment.Exit(0);

        }

        /// <summary>
        /// 输出不符合规范编码文件
        /// </summary>
        private static string PintnoConformEncodingFile(EncodingScrutatorFolder encodingScrutatorFolder, string white = "", StringBuilder str = null)
        {

            if (str == null)
            {
                str = new StringBuilder();
            }
            foreach (var temp in encodingScrutatorFolder.Folder)
            {
                PintnoConformEncodingFile(temp, white + temp.Name + "/", str);
            }
            foreach (var temp in encodingScrutatorFolder.File)
            {
                str.Append(white);
                str.Append(temp.File.Name + " ");
                if (temp.Ignore)
                {
                    str.Append("文件忽略");


                }
                else
                {
                    str.Append("编码 " + temp.Encoding.EncodingName + " 置信度 " + temp.ConfidenceCount + " ");
                    if (encodingScrutatorFolder.SitpulationEncodingSetting.ConformtotheDefaultEncoding(temp.Encoding))
                    {
                        str.Append("文件符合规范");
                        _illicitFile.Add(temp);
                    }
                    else
                    {
                        str.Append("文件不符合规范");
                    }
                    //str.Append(
                    //    encodingScrutatorFolder.SitpulationEncodingSetting.ConformtotheDefaultEncoding(temp.Encoding)
                    //        ? "文件符合规范"
                    //        : "文件不符合规范");
                }
                str.Append("\r\n");
            }
            return str.ToString();
        }

        //private static bool _illicitFile=false;//存在不合法文件

        private static List<EncodingScrutatorFile> _illicitFile = new List<EncodingScrutatorFile>();

        private static string Print(EncodingScrutatorFolder encodingScrutatorFolder, string white = "", StringBuilder str = null)
        {
            //Console.WriteLine("包含文件");
            //foreach (var temp in encodingScrutatorFolder.IncludeFileSetting.IncludeWildcardFile)
            //{
            //    Console.WriteLine(temp);
            //}

            //Console.WriteLine("不包含文件");
            //foreach (var temp in encodingScrutatorFolder.InspectFileWhiteListSetting.FileWhiteList)
            //{
            //    Console.WriteLine(temp);
            //}

            //foreach (var temp in encodingScrutatorFolder.InspectFileWhiteListSetting.FolderWhiteList)
            //{
            //    Console.WriteLine(temp);
            //}

            if (str == null)
            {
                str = new StringBuilder();
            }
            foreach (var temp in encodingScrutatorFolder.Folder)
            {
                Print(temp, white + temp.Name + "/", str);
            }
            //StringBuilder str=new StringBuilder();

            foreach (var temp in encodingScrutatorFolder.File)
            {
                str.Append(white);
                str.Append(temp.File.Name + " ");
                if (temp.Ignore)
                {
                    str.Append("文件忽略");
                }
                else
                {
                    str.Append("编码 " + temp.Encoding.EncodingName + " 置信度 " + temp.ConfidenceCount);
                }
                str.Append("\r\n");
            }

            //using (StreamWriter stream=new StreamWriter(
            //    new FileStream("E:\\1.txt",FileMode.Create)))
            //{
            //    stream.Write(str.ToString());
            //}

            //Console.WriteLine(str.ToString());
            return str.ToString();
        }

 

        //private static void ReadFile(EncodingScrutatorFile encoding)
        //{
        //    using (StreamReader stream=new StreamReader(encoding.File.Open(FileMode.Open),Encoding.UTF8))
        //    {
        //        string str = stream.ReadToEnd();
        //    }

        //    using (StreamReader stream = new StreamReader(encoding.File.Open(FileMode.Open), Encoding.GetEncoding("GBK")))
        //    {
        //        string str = stream.ReadToEnd();
        //    }
        //}

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

        ///// <summary>
        ///// 写入编码各种
        ///// </summary>
        //private static void EncodingWrite()
        //{
        //    string str = "这是{0}编码";
        //    List<Encoding> encoding=new List<Encoding>()
        //    {
        //        Encoding.ASCII,
        //        Encoding.BigEndianUnicode,
        //        Encoding.UTF8,
        //        Encoding.GetEncoding("gbk"),

        //    };

        //    string file = "E:\\程序\\公司\\EncodingNormalior\\EncodingNormalior\\textFile\\";
        //    foreach (var temp in encoding)
        //    {
        //        using (StreamWriter stream =new StreamWriter(
        //            new FileStream(file+temp.EncodingName+".txt",FileMode.OpenOrCreate),temp))
        //        {
        //            stream.Write(str, temp);
        //        }
        //    }
        //}
    }


}
