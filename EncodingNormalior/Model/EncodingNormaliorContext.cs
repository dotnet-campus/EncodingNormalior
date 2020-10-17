namespace EncodingNormalior.Model
{
    class EncodingNormaliorContext
    {
        /// <summary>
        /// 白名单的文件后缀名，这部分的文件忽略文件编码
        /// </summary>
        public const string WhiteList =
@".git\
.vs\
obj\
bin\
packages\
*.png
*.jpg
*.tmp
*.exe
*.mp4
*.snk
*.pfx
*.zip
*.rar
*.7z
*.cer";

        /// <summary>
        /// 常用文本后缀
        /// </summary>
        public const string TextFileSuffix = @"txt
cs
java
c
cpp
md
csproj
json
asp
bat 
vbs
cmd 
html
htm
csv
ini
log
xaml
xml";
    }
}