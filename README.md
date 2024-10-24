# 编码检测和修改工具

| Appveyor Build | GitHub Action |
| -- | -- |
|[![Build status](https://ci.appveyor.com/api/projects/status/ypcqbbc7bsexnfog?svg=true)](https://ci.appveyor.com/project/lindexi_gd/encodingnormalior)| ![](https://github.com/dotnet-campus/EncodingNormalior/workflows/.NET%20Core/badge.svg) |

| Package Name                   | Release (NuGet) | 
|--------------------------------|-----------------|
|  `dotnetCampus.EncodingNormalior`       |[![NuGet](https://img.shields.io/nuget/v/dotnetCampus.EncodingNormalior.svg)](https://www.nuget.org/packages/dotnetCampus.EncodingNormalior/)|
|  `lindexi.src.EncodingUtf8AndGBKDifferentiater`      |[![NuGet](https://img.shields.io/nuget/v/lindexi.src.EncodingUtf8AndGBKDifferentiater.svg)](https://www.nuget.org/packages/lindexi.src.EncodingUtf8AndGBKDifferentiater/)|

在开发中经常遇到编码不一致的文件，而如果这些文件包含需要显示的字符串，就会导致在乱码。所以需要一个工具可以自动检测工程、文件夹内所有文本的编码，并可以规范所有文件编码。

工具要求可以设置规定的编码，如果文件的编码不是规定的编码，用户可以选择把文件的编码转换为规定的编码。

用户可以设置白名单，白名单可以让某些文件或文件夹的文件不检测。

默认是检测所有的文本文件，但是因为没有一个好的算法，所以在无法判断一个文件是文本时，会去查看用户是不是设置了一定包含某个文件后缀，一旦文件在用户的包含后缀中，那么就会去检测文件编码。

程序首先是检查文件是否在白名单，如果不在，再检测是否包含文件后缀，如果没有，再检测文件是否是文本，如果是的话，就检查。

<!--more-->



首先是告诉大家如何使用工具。

## 工具的使用

### 命令行使用

命令行使用参见：[编码工具——命令行](./EncodingNormalior/README.md)

### 插件使用

首先是下载插件，插件可以到 [https://visualstudiogallery.msdn.microsoft.com/a5f50c64-1b75-4f7a-97fd-9545747c506a](https://marketplace.visualstudio.com/items?itemName=lindexigd.vs-extension-18109) 下载，也可以在 [VS 插件网](https://marketplace.visualstudio.com/vs) 搜索 `Encoding` 就可以找到我的工具啦。可以说我翻遍了整个插件网，都没有找到我这个功能的插件（这句话是在2017年说的），但是还是找到了一些有用的插件。

![](http://cdn.lindexi.site/8f464be7-2358-45f4-b6cd-eae32c47a878201727162028.jpg)

打开 Visual Sutido ，在安装完 [编码规范工具](https://marketplace.visualstudio.com/items?itemName=lindexigd.vs-extension-18109) ，可以看到菜单多了 选项 EncodingNormalizer

![](http://cdn.lindexi.site/lindexi%2F2019461567484)

然后来说下我做的功能。

#### 修改当前文件编码

在 2.6 版本新添加的功能是修改当前打开的文件的编码，即使这个文件不在当前的项目里面

因为现在没有一个方法可以知道一个文件是什么编码，所以可以在插件自己手动选当前文件的编码，这样可以解决识别编码错误

![](http://cdn.lindexi.site/lindexi%2F202022911598327.jpg)

选择当前的文件的编码，和需要转换的文件的编码，然后点击 Convert 就可以转换了

<!-- ![](image/VisualStudio 编码规范工具 2.6 修改当前文件编码/VisualStudio 编码规范工具 2.6 修改当前文件编码5.png) -->

![](http://cdn.lindexi.site/lindexi%2F201946153140370)

现在能支持的转换的编码是带符号的 Utf-8 和 GBK 编码

#### 设置

点击菜单 EncodingNormailzer ，选择 Setting 可以看到下面界面

<!-- ![](image/VisualStudio 编码规范工具 2.6 修改当前文件编码/VisualStudio 编码规范工具 2.6 修改当前文件编码1.png) -->

![](http://cdn.lindexi.site/lindexi%2F201946151729613)

首先是可以忽略一些文件或文件夹，默认是忽略一些不是文本的文件和 bin、obj、git文件夹，注意，千万不要去转换 git 文件夹的代码。

然后我们可以设置编码，现在做的是 Utf8 、GBK、Unicode的编码，如果检测工程存在文件的编码和我们设置的不一样，就会提示去转换。

因为 Ascii 的文件，存放为 GBK 和 UTF8不带签名是无法区分的，所以忽略 ASCII 编码文件。

因为对 Unicode-16 的文件是无法使用判断存在 '\0' 来区分文件是不是文本，所以，对于某些文件还是自己手动添加是否一定检测，对于没有被添加到一定需要检测的文件，先判断他是不是文本，如果是的话，就检测。

设置保存在 `我的文档\EncodingNormalizer\Account.json` 文件

#### 检查编码

然后在打开完工程，注意要加载完成才使用。

点击 Conform solution encoding ，自动检测方案所有工程的文件编码，如果发现所有的编码都符合规范，那么弹出窗口说所有文件都符合规范。如果有文件不符合规范，那么提示用户是否转换。


<!-- ![](image/VisualStudio 编码规范工具 2.6 修改当前文件编码/VisualStudio 编码规范工具 2.6 修改当前文件编码2.png) -->

![](http://cdn.lindexi.site/lindexi%2F201946151823827)

找到所有不符合规范的文件，可以一键点击转换

![](http://cdn.lindexi.site/lindexi%2F20194615184676)

<!-- ![](image/VisualStudio 编码规范工具 2.6 修改当前文件编码/VisualStudio 编码规范工具 2.6 修改当前文件编码3.png) -->

## 和我组队做工具

这个工具相信是大家比较需要的，所以我就做了这个工具。做这个工具最难的地方在于判断文件编码，和如何做vs扩展两个。如果大家也想做一个差不多的东西，可以参见开发过程中使用的技术：[C＃ 判断文件编码](https://blog.lindexi.com/post/C-%E5%88%A4%E6%96%AD%E6%96%87%E4%BB%B6%E7%BC%96%E7%A0%81.html )  [VisualStudio 扩展开发](https://blog.lindexi.com/post/VisualStudio-%E6%89%A9%E5%B1%95%E5%BC%80%E5%8F%91.html )

我把工具放在 github ： [https://github.com/dotnet-campus/EncodingNormalior](https://github.com/dotnet-campus/EncodingNormalior)

好像我的项目名称 编码规范工具 是 EncodingNormalizer ，写错了，但是不想改。

我还把他放在 vs 扩展库，可以到 [https://marketplace.visualstudio.com/](https://marketplace.visualstudio.com/) 下载。

来说下如何使用我的项目：

我的项目有类库 EncodingNormalior 和插件 EncodingNormalizerVsx。

如果想运行类库，不需要做任何修改。如果想运行插件，需要修改属性->调试，使用外部程序，里面写`C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe` 我现在是 VisualStudio 2015 所以是在`Visual Studio 14.0`

然后修改命令行参数:`/rootsuffix Exp`

然后就可以运行。

<!-- 现在发在VS插件网站，可以在 下载 -->

如果希望开发这个项目，可以去fork我的github库。

如果现在使用的是 visualStudio 2017 企业版，那么把外部程序修改为`C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe`

现在使用 VisualStudio 2019 社区版，将外部程序修改为 `C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe`

<!-- 

如果本地存在 textFileSuffix.txt 那么读取配置，哪些符合后缀名的文件要判断编码。如果本地不存在，使用默认配置。读取的值放在 IncludeFileSetting.TextFileSuffix 

如果本地存在 includeFile.txt ，那么读取配置，哪些文件是要判断编码，支持通配。如果本地不存在，使用默认配置。 读取的值放在  IncludeFileSetting.TextFileSuffix

如果本地存在 WhiteList.txt ，那么读取配置，哪些文件是忽略的。如果本地不存在，使用默认配置。读取的值放在 InspectFileWhiteListSetting.DefaultWhiteList 

白名单可以忽略文件夹，文件夹不能使用通配。如果本地不存在，使用默认配置。

因为用到命令行，于是解析命令行使用的是 http://www.cnblogs.com/linxuanchen/p/c-sharp-command-line-argument-parser.html 大神写的方法。 -->
