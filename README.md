# 编码检测和修改工具

在开发中经常遇到编码不一致的文件，导致在乱码。所以需要一个工具可以自动检测工程、文件夹内所有文本的编码，并可以规范所有文件编码。

工具要求可以设置规定的编码，如果文件的编码不是规定的编码，用户可以选择把文件的编码转换为规定的编码。

用户可以设置白名单，某些文件或文件夹的文件不检测。

<!--more-->

我就做了这个工具，使用的技术：[判断文件编码](http://lindexi.oschina.io/lindexi/post/C-%E5%88%A4%E6%96%AD%E6%96%87%E4%BB%B6%E7%BC%96%E7%A0%81/) [扩展开发](http://lindexi.oschina.io/lindexi/post/VisualStudio 扩展开发/)


好像我的项目名称 编码规范工具 是 EncodingNormalizer ，写错了，但是不想改。

来说下如何使用我的项目：

我的项目有类库 EncodingNormalior 和插件 EncodingNormalizerVsx。

如果想运行类库，不需要做任何修改。如果想运行插件，需要修改属性->调试，使用外部程序，里面写`C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe` 我现在是 VisualStudio 2015 所以是在`Visual Studio 14.0`

然后修改命令行参数:`/rootsuffix Exp`

然后就可以运行。



<!-- 

如果本地存在 textFileSuffix.txt 那么读取配置，哪些符合后缀名的文件要判断编码。如果本地不存在，使用默认配置。读取的值放在 IncludeFileSetting.TextFileSuffix 

如果本地存在 includeFile.txt ，那么读取配置，哪些文件是要判断编码，支持通配。如果本地不存在，使用默认配置。 读取的值放在  IncludeFileSetting.TextFileSuffix

如果本地存在 WhiteList.txt ，那么读取配置，哪些文件是忽略的。如果本地不存在，使用默认配置。读取的值放在 InspectFileWhiteListSetting.DefaultWhiteList 

白名单可以忽略文件夹，文件夹不能使用通配。如果本地不存在，使用默认配置。

因为用到命令行，于是解析命令行使用的是 http://www.cnblogs.com/linxuanchen/p/c-sharp-command-line-argument-parser.html 大神写的方法。 -->
