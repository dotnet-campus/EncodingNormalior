# 编码规范工具——命令行

## 命令行使用

编码规范工具使用方法：

### 必选命令：

输入要检测的文件夹或*csproj文件

#### 输入格式：

`输入文件夹      -f 文件夹路径`

### 可选命令：
 
#### 输入格式：
    
 - 必须包含的文件    `--IncludeFile 文件路径  ` 

 - 文件白名单       `--WhiteList   文件路径`

 - 规定的编码       `-E            Encoding`

注意：    Encoding 包含 utf8、 gbk、 ascii、utf16、BigEndianUnicode

程序从输入的文件夹读取配置，越靠近文本的配置优先度越高。
如果不存在配置，使用默认配置。

 - 白名单配置名称：        WhiteList.txt

 - 必须包含的文件配置名称： IncludeFile.txt

要求配置编码是 UTF8