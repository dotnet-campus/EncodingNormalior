# 编码规范工具——命令行

## 安装

通过 dotnet tool 安装

确定 dotnet 版本在 3.1 以上，可从[https://dotnet.microsoft.com/](https://dotnet.microsoft.com/)更新到最新版本

在命令行输入下面命令作为全局工具安装

```csharp
dotnet tool install -g dotnetCampus.EncodingNormalior
```

## 命令行使用

编码规范工具使用方法：

### 必选命令：

输入要检测的文件夹或csproj文件

```
EncodingNormalior -f E:\lindexi\EncodingNormalior
```

#### 输入格式：

`输入文件夹      -f 文件夹路径`

### 可选命令：
 
#### 输入格式：
    
 - 必须包含的文件    `--IncludeFile 文件路径`

 - 文件白名单       `--WhiteList   文件路径`

 - 规定的编码       `-E            Encoding`     默认是 Utf8 编码

 - 是否尝试修复编码  `--TryFix      [true|false]` 设置true将尝试自动修复，默认为不自动修复

注意：    Encoding 包含 utf8、 gbk、 ascii、utf16、BigEndianUnicode

程序从输入的文件夹读取配置，越靠近文本的配置优先度越高。
如果不存在配置，使用默认配置。

 - 白名单配置名称：        WhiteList.txt

 - 必须包含的文件配置名称： IncludeFile.txt

要求编码是 UTF8

## 配置文件格式

配置文件有白名单文件和指定文本的文件后缀。

白名单文件是让软件可以忽略某些文件或文件夹，如 obj 这些文件夹。

因为判断一个文件是文本的算法，还没有存在一个可以绝对判断的算法，所以依靠人工添加。

软件检查文件是否需要检测的顺序是：

1. 判断文件是否在白名单中，如果是，检查结束，不进行检测编码。如果不是，继续。

2. 判断文件是否在 `必须包含的文件配置` 中，如果是，检查结束，进行检测编码。如果不是，继续。

3. 判断文件是否是文本，如果是，进行检测编码。

对于 Utf-32 的文件，几乎不存在一个算法可以判断他是文本，除非他带 BOM 。

关于文件编码相关，如果先了解更多，戳此链接[C＃ 判断文件编码](https://blog.lindexi.com/post/C-%E5%88%A4%E6%96%AD%E6%96%87%E4%BB%B6%E7%BC%96%E7%A0%81.html)

### 白名单文件格式

白名单包括文件夹白名单和文件白名单。

白名单文件以行作为分割，一行写一个文件或文件夹。

其中文件夹白名单的格式为 `文件名 + \\ `

如 obj 文件夹添加到白名单应该写 `obj\\ `

文件夹不支持通配符。

注意：文件夹的`\\`也可以写为`/`但是不建议。

文件白名单格式为 `文件`

如 lindexi.png 文件夹添加到白名单应该写 `lindexi.png`

文件支持通配符，如忽略所有的 png 文件，可以写 `*.png`

文件通配符和 MS （Microsoft） 通配符格式相同。

不支持指定文件夹中的文件。如 `obj\\lindexi.txt`。

默认[白名单](./Resource/WhiteList.txt):https://github.com/dotnet-campus/EncodingNormalior/blob/master/EncodingNormalior/Resource/WhiteList.txt

### 包含文件的配置文件格式

包含文件的配置以行做分割。

一行包括一个包含的文件。

文件支持通配符，通配符和 MS （Microsoft） 通配符格式相同。


例：

```csharp
lindexi.txt
*.lindexitxt
lindexi.md
lindexi.dox
lindexi*.txt  
```

注意：包含的文件中不支持文件夹路径。

