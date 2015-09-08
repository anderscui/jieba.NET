jieba.NET是[jieba中文分词](https://github.com/fxsjy/jieba)的.NET版本。

当前版本为0.37.1，基于jieba 0.37，目标是提供与jieba一致的功能与接口，但以后可能会在jieba基础上提供其它扩展功能。关于jieba的实现思路，可以看看[这篇wiki](https://github.com/anderscui/jieba.NET/wiki/%E7%90%86%E8%A7%A3%E7%BB%93%E5%B7%B4%E5%88%86%E8%AF%8D)里提到的资料。

## 特点

* 支持三种分词模式：
    - 精确模式，试图将句子最精确地切开，适合**文本分析**；
    - 全模式，把句子中所有的可以成词的词语都扫描出来, **速度非常快，但是不能解决歧义**。具体来说，分词过程不会借助于词频查找最大概率路径，亦不会使用HMM；
    - 搜索引擎模式，在精确模式的基础上，对长词再次切分，提高召回率，**适合用于搜索引擎分词**。
* 支持**繁体分词**
* 支持添加自定义词典和自定义词
* MIT 授权协议

## 算法

* 基于前缀词典实现高效的词图扫描，生成句子中汉字所有可能成词情况所构成的有向无环图 (DAG)
* 采用了动态规划查找最大概率路径, 找出基于词频的最大切分组合
* 对于未登录词，采用了基于汉字成词能力的HMM模型，使用了Viterbi算法

## 安装和配置

当前版本基于.NET Framework 4.5，支持NuGet方式安装：

```shell
PM> Install-Package jieba.NET
```

安装之后，在packages\jieba.NET目录下可以看到Resources目录，这里面是jieba.NET运行所需的词典及其它数据文件，最简单的配置方法是将整个Resources目录拷贝到程序集所在目录，这样jieba.NET会使用内置的默认配置值。如果希望将这些文件放在其它位置，则要在app.config或web.config中添加如下的配置项：

```xml
<appSettings>
    <add key="MainDictFile" value="Resources\dict.txt" />
    <add key="ProbTransFile" value="Resources\prob_trans.json" />
    <add key="ProbEmitFile" value="Resources\prob_emit.json" />

    <add key="PosProbStartFile" value="Resources\pos_prob_start.json" />
    <add key="PosProbTransFile" value="Resources\pos_prob_trans.json" />
    <add key="PosProbEmitFile" value="Resources\pos_prob_emit.json" />
    <add key="CharStateTabFile" value="Resources\char_state_tab.json" />

    <add key="StopWordsFile" value="Resources\stopwords.txt" />
    <add key="IdfFile" value="Resources\idf.txt" />
</appSettings>
```

## 主要功能

### 1. 分词

* `JiebaSegmenter.Cut`方法接受三个输入参数，text为待分词的字符串；cutAll指定是否采用全模式；hmm指定使用是否使用hmm模型切分未登录词；返回类型为`IEnumerable<string>`
* `JiebaSegmenter.CutForSearch`方法接受两个输入参数，text为待分词的字符串；hmm指定使用是否使用hmm模型；返回类型为`IEnumerable<string>`

代码示例

```c#
var segmenter = new JiebaSegmenter();
var segments = segmenter.Cut("我来到北京清华大学", cutAll: true);
Console.WriteLine("【全模式】：{0}", string.Join("/ ", segments));

segments = segmenter.Cut("我来到北京清华大学");  // 默认为精确模式
Console.WriteLine("【精确模式】：{0}", string.Join("/ ", segments));

segments = segmenter.Cut("他来到了网易杭研大厦");  // 默认为精确模式，同时也使用HMM模型
Console.WriteLine("【新词识别】：{0}", string.Join("/ ", segments));

segments = segmenter.CutForSearch("小明硕士毕业于中国科学院计算所，后在日本京都大学深造"); // 搜索引擎模式
Console.WriteLine("【搜索引擎模式】：{0}", string.Join("/ ", segments));

segments = segmenter.Cut("结过婚的和尚未结过婚的");
Console.WriteLine("【歧义消除】：{0}", string.Join("/ ", segments));
```

输出

```
【全模式】：我/ 来到/ 北京/ 清华/ 清华大学/ 华大/ 大学
【精确模式】：我/ 来到/ 北京/ 清华大学
【新词识别】：他/ 来到/ 了/ 网易/ 杭研/ 大厦
【搜索引擎模式】：小明/ 硕士/ 毕业/ 于/ 中国/ 科学/ 学院/ 科学院/ 中国科学院/ 计算/ 计算所/ ，/ 后/ 在/ 日本/ 京都/ 大学/ 日本京都大学/ 深造
【歧义消除】：结过婚/ 的/ 和/ 尚未/ 结过婚/ 的
```


### 2. 添加自定义词典

#### 加载词典

* 开发者可以指定自己自定义的词典，以便包含jieba词库里没有的词。虽然jieba有新词识别能力，但是自行添加新词可以保证更高的正确率
* `JiebaSegmenter.LoadUserDict("user_dict_file_path")`
* 词典格式与主词典格式相同，即一行包含：词、词频（可省略）、词性（可省略），用空格隔开
* 词频省略时，分词器将使用自动计算出的词频保证该词被分出

如

```
创新办 3 i
云计算 5
凱特琳 nz
台中
机器学习 3
```

#### 调整词典

* 使用`JiebaSegmenter.AddWord(word, freq=0, tag=null)`可添加一个新词，或调整已知词的词频；若`freq`不是正整数，则使用自动计算出的词频
* 使用`JiebaSegmenter.DeleteWord(word)`可移除一个词，使其不能被分出来

### 3. 关键词提取

#### 基于TF-IDF算法的关键词提取

* `JiebaNet.Analyser.TfidfExtractor.ExtractTags(string text, int count = 20, IEnumerable<string> allowPos = null)`可从指定文本中抽取出关键词。
* `JiebaNet.Analyser.TfidfExtractor.ExtractTagsWithWeight(string text, int count = 20, IEnumerable<string> allowPos = null)`可从指定文本中**抽取关键词的同时得到其权重**。
* 关键词抽取基于逆向文件频率（IDF），组件内置一个IDF语料库，可以配置为其它自定义的语料库。
* 关键词抽取会过滤停用词（Stop Words），组件内置一个极小的语料库，建议根据需要配置为其它自定义的语料库。

#### 基于TextRank算法的关键词抽取

* `JiebaNet.Analyser.TextRankExtractor`与`TfidfExtractor`相同的接口。需要注意的是，`TextRankExtractor`默认情况下只提取名词和动词。

### 4. 词性标注

* `JiebaNet.Segmenter.PosSeg.PosSegmenter`类可以在分词的同时，为每个词添加词性标注。
* 词性标注采用和ictclas兼容的标记法，关于ictclas和jieba中使用的标记法列表，请参考：[词性标记](https://gist.github.com/luw2007/6016931)。

```c#
var posSeg = new PosSegmenter();
var s = "一团硕大无朋的高能离子云，在遥远而神秘的太空中迅疾地飘移";

var tokens = posSeg.Cut(s);
Console.WriteLine(string.Join(" ", tokens.Select(token => string.Format("{0}/{1}", token.Word, token.Flag))));
```

```
一团/m 硕大无朋/i 的/uj 高能/n 离子/n 云/ns ，/x 在/p 遥远/a 而/c 神秘/a 的/uj 太空/n 中/f 迅疾/z 地/uv 飘移/v
```

### 5. Tokenize：返回词语在原文的起止位置

* 默认模式

```c#
var segmenter = new JiebaSegmenter();
var s = "永和服装饰品有限公司";
var tokens = segmenter.Tokenize(s);
foreach (var token in tokens)
{
    Console.WriteLine("word {0,-12} start: {1,-3} end: {2,-3}", token.Word, token.StartIndex, token.EndIndex);
}
```

```
word 永和           start: 0   end: 2
word 服装           start: 2   end: 4
word 饰品           start: 4   end: 6
word 有限公司         start: 6   end: 10
```

* 搜索模式

```c#
var segmenter = new JiebaSegmenter();
var s = "永和服装饰品有限公司";
var tokens = segmenter.Tokenize(s, TokenizerMode.Search);
foreach (var token in tokens)
{
    Console.WriteLine("word {0,-12} start: {1,-3} end: {2,-3}", token.Word, token.StartIndex, token.EndIndex);
}
```

```
word 永和           start: 0   end: 2
word 服装           start: 2   end: 4
word 饰品           start: 4   end: 6
word 有限           start: 6   end: 8
word 公司           start: 8   end: 10
word 有限公司         start: 6   end: 10
```

### 6. 并行分词（暂未实现）
