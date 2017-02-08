using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public const string NoFolderArgException = "不存在文件夹参数";

        private static void ParseCommand(CommandLineArgumentParser arguments)
        {
            ConformCommand(arguments);
            var folder = arguments.Get(FolderCommand).Next;
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentCommadnException(NoFolderArgException);
            }
            if (!Directory.Exists(folder))
            {
                throw new ArgumentCommadnException("不存在文件夹" + folder);
            }

            EncodingScrutatorFolder encodingScrutatorFolder = new EncodingScrutatorFolder(new DirectoryInfo(folder));
            if (arguments.Has(IncludeFileCommand))
            {
                var includeFile = arguments.Get(IncludeFileCommand).Take();
                var wildCardFile = /*IncludeFileSetting.ReadIncludeFile(includeFile);*/
                    new FileInfo(includeFile);

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

            encodingScrutatorFolder.InspectFolderEncoding();
            Console.WriteLine(PintnoConformEncodingFile(encodingScrutatorFolder));
            if (_illicitFile.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                str.Append($"存在不符合规范文件：  {_illicitFile.Count}\r\n");
                foreach (var temp in _illicitFile)
                {
                    str.Append(temp.File.FullName + "\r\n");
                }
                throw new Exception(str.ToString());
            }
            else
            {
                Console.WriteLine("恭喜你，没有存在不规范文件\r\n");
            }
        }

        private static void ConformCommand(CommandLineArgumentParser arguments)
        {
            List<string> command = new List<string>()
            {
                IncludeFileCommand,
                WhiteListCommand,
                EncodingCommand,
                FolderCommand,
            };


            foreach (var arg in arguments.GetEnumerator().Where(temp => ((string)temp).StartsWith("-")))
            {
                if (command.All(temp => !temp.Equals(arg)))
                {
                    throw new ArgumentCommadnException("存在无法识别参数" + arg);
                }
            }


            if (!arguments.Has(FolderCommand))
            {
                throw new ArgumentCommadnException("需要包含要检测文件夹");
            }
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
                catch (ArgumentCommadnException e)
                {
                    Console.WriteLine(e.Message + "\r\n" + Usage);
                    Environment.Exit(-1);
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
                //str.Append(white);
                str.Append($"文件名： {white}{temp.File.Name}\r\n");
                if (temp.Ignore)
                {
                    str.Append("文件忽略\r\n");
                }
                else
                {
                    str.Append($"编码　:{temp.Encoding.EncodingName.PadRight(10)} {(temp.Encoding.Equals(Encoding.ASCII) ? "" : $"置信度:{temp.ConfidenceCount}")} \r\n");
                    //str.Append("编码　:" + temp.Encoding.EncodingName + " 置信度 " + temp.ConfidenceCount + " ");
                    str.Append("状态:");
                    if (encodingScrutatorFolder.SitpulationEncodingSetting.ConformtotheDefaultEncoding(temp.Encoding))
                    {
                        str.Append("文件符合规范");
                        _illicitFile.Add(temp);
                    }
                    else
                    {
                        str.Append("文件不符合规范");
                    }
                    str.Append("\r\n");

                    //str.Append(
                    //    encodingScrutatorFolder.SitpulationEncodingSetting.ConformtotheDefaultEncoding(temp.Encoding)
                    //        ? "文件符合规范"
                    //        : "文件不符合规范");
                }
                str.Append("\r\n");
            }
            return str.ToString();
        }


        private static List<EncodingScrutatorFile> _illicitFile = new List<EncodingScrutatorFile>();

        //private static string Print(EncodingScrutatorFolder encodingScrutatorFolder, string white = "", StringBuilder str = null)
        //{
        //    //Console.WriteLine("包含文件");
        //    //foreach (var temp in encodingScrutatorFolder.IncludeFileSetting.IncludeWildcardFile)
        //    //{
        //    //    Console.WriteLine(temp);
        //    //}

        //    //Console.WriteLine("不包含文件");
        //    //foreach (var temp in encodingScrutatorFolder.InspectFileWhiteListSetting.FileWhiteList)
        //    //{
        //    //    Console.WriteLine(temp);
        //    //}

        //    //foreach (var temp in encodingScrutatorFolder.InspectFileWhiteListSetting.FolderWhiteList)
        //    //{
        //    //    Console.WriteLine(temp);
        //    //}

        //    if (str == null)
        //    {
        //        str = new StringBuilder();
        //    }
        //    foreach (var temp in encodingScrutatorFolder.Folder)
        //    {
        //        Print(temp, white + temp.Name + "/", str);
        //    }
        //    //StringBuilder str=new StringBuilder();

        //    foreach (var temp in encodingScrutatorFolder.File)
        //    {
        //        str.Append(white);
        //        str.Append(temp.File.Name + " ");
        //        if (temp.Ignore)
        //        {
        //            str.Append("文件忽略");
        //        }
        //        else
        //        {
        //            str.Append("编码 " + temp.Encoding.EncodingName + " 置信度 " + temp.ConfidenceCount);
        //        }
        //        str.Append("\r\n");
        //    }

        //    //using (StreamWriter stream=new StreamWriter(
        //    //    new FileStream("E:\\1.txt",FileMode.Create)))
        //    //{
        //    //    stream.Write(str.ToString());
        //    //}

        //    //Console.WriteLine(str.ToString());
        //    return str.ToString();
        //}
    }
}
