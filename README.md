# 编码检测和修改工具


自动检测工程、文件夹内所有文本的编码。

可以设置规定的编码，如果文件的编码不是规定的编码，用户可以选择把文件的编码转换为规定的编码。

用户可以设置白名单，某些文件或文件夹的文件不检测。

<!--more-->

注意：本文还处于记录状态


好像我的名称是 EncodingNormalizer ，但是不想改。

如果本地存在 textFileSuffix.txt 那么读取配置，哪些符合后缀名的文件要判断编码。如果本地不存在，使用默认配置。读取的值放在 IncludeFileSetting.TextFileSuffix 

如果本地存在 includeFile.txt ，那么读取配置，哪些文件是要判断编码，支持通配。如果本地不存在，使用默认配置。 读取的值放在  IncludeFileSetting.TextFileSuffix

如果本地存在 WhiteList.txt ，那么读取配置，哪些文件是忽略的。如果本地不存在，使用默认配置。读取的值放在 InspectFileWhiteListSetting.DefaultWhiteList 

白名单可以忽略文件夹，文件夹不能使用通配。如果本地不存在，使用默认配置。

因为用到命令行，于是解析命令行使用的是 http://www.cnblogs.com/linxuanchen/p/c-sharp-command-line-argument-parser.html 大神写的方法。
