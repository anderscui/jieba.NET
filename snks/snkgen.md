# SNK 文件生成说明

.net 强命名最简单的方法是通过 snk 在编译的时候直接强命名。

为了安全原因，不提供秘钥对 文件 JiebaNet.Analyser.snk、JiebaNet.Segmenter.snk

本不应提供 JiebaNet.Segmenter.Test.snk ，
但考虑到 internalvisiable的特性，故把 JiebaNet.Segmenter.Test.snk 包含进来。

## 生成秘钥对步骤

1. 以管理员身份 打开 VS工具命令提示
1. CD到当前目录
1. 使用SN命令

## 当前项目需要手动生成的秘钥对：

* SN -k JiebaNet.Analyser.snk
* SN -k JiebaNet.Segmenter.snk

## 测试项目的秘钥对：
* SN -k JiebaNet.Segmenter.Test.snk

以管理员身份执行 cmd_vs.bat 可迅速执行秘钥步骤1、2步。

查看 Test 项目公钥：
* SN –p JiebaNet.Segmenter.Test.snk JiebaNet.Segmenter.PublicKey
* SN –tp JiebaNet.Segmenter.PublicKey