# 编码检测和修改工具

在开发中经常遇到编码不一致的文件，导致在乱码。所以需要一个工具可以自动检测工程、文件夹内所有文本的编码，并可以规范所有文件编码。

工具要求可以设置规定的编码，如果文件的编码不是规定的编码，用户可以选择把文件的编码转换为规定的编码。

用户可以设置白名单，白名单可以让某些文件或文件夹的文件不检测。

默认是检测所有的文本文件，但是因为没有一个好的算法，所以在无法判断一个文件是文本时，会去查看用户是不是设置了一定包含某个文件后缀，一旦文件在用户的包含后缀中，那么就会去检测文件编码。

程序首先是检查文件是否在白名单，如果不在，再检测是否包含文件后缀，如果没有，再检测文件是否是文本，如果是的话，就检查。

<!--more-->

这个工具相信是比较需要的，所以我就做了这个工具。做这个工具最难在于判断文件编码，和如何做vs扩展。如果大家也想做一个差不多的东西，可以参见博客使用的技术：[判断文件编码](http://lindexi.oschina.io/lindexi/post/C-%E5%88%A4%E6%96%AD%E6%96%87%E4%BB%B6%E7%BC%96%E7%A0%81/) [扩展开发](http://lindexi.oschina.io/lindexi/post/VisualStudio 扩展开发/)

我把工具放在 github ：https://github.com/lindexi/EncodingNormalior

好像我的项目名称 编码规范工具 是 EncodingNormalizer ，写错了，但是不想改。

我还把他放在 vs 扩展库，还没有发出来，于是我就不发链接。

来说下如何使用我的项目：

我的项目有类库 EncodingNormalior 和插件 EncodingNormalizerVsx。

如果想运行类库，不需要做任何修改。如果想运行插件，需要修改属性->调试，使用外部程序，里面写`C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe` 我现在是 VisualStudio 2015 所以是在`Visual Studio 14.0`

然后修改命令行参数:`/rootsuffix Exp`

然后就可以运行。

<!-- 现在发在VS插件网站，可以在 下载 -->

如果希望开发这个项目，可以去fork我的github库。

<!-- 

如果本地存在 textFileSuffix.txt 那么读取配置，哪些符合后缀名的文件要判断编码。如果本地不存在，使用默认配置。读取的值放在 IncludeFileSetting.TextFileSuffix 

如果本地存在 includeFile.txt ，那么读取配置，哪些文件是要判断编码，支持通配。如果本地不存在，使用默认配置。 读取的值放在  IncludeFileSetting.TextFileSuffix

如果本地存在 WhiteList.txt ，那么读取配置，哪些文件是忽略的。如果本地不存在，使用默认配置。读取的值放在 InspectFileWhiteListSetting.DefaultWhiteList 

白名单可以忽略文件夹，文件夹不能使用通配。如果本地不存在，使用默认配置。

因为用到命令行，于是解析命令行使用的是 http://www.cnblogs.com/linxuanchen/p/c-sharp-command-line-argument-parser.html 大神写的方法。 -->
