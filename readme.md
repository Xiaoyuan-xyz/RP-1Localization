刚刚接触KSP的Patch翻译，有很多地方搞不明白，但为了以防忘记，还是记录一些用到的东西，来规范自己的翻译

在Localization下按照不同Mod的路径造一个同样的路径，如果原位置有一个xx.cfg文件 翻译时就在对应位置创建一个zh-cn-xx.cfg文件。
如果打算把某一个目录比如xx/下的所有文件翻译成一个patch，就起名成zh-cn.cfg。（暂定）

如果要在优先级FOR[RealismOverhaulTankTypes]前修改，就用FOR[RealismOverhaulAAATankTypes]之后就用AFTER[RealismOverhaulZZZTankTypes]。
如果要在FOR[RealismOverhaul]优先级后用FOR修改，就用FOR[RealismOverhaulXYZ]。
如果没有优先级，就也不用优先级或者用FOR[XYZLocalization]（哪个好一些？）

## 语法

[教程参考](hhttps://www.bilibili.com/opus/795218190163509272)

```
[操作符][[ConfigNode][ConfigNode 的 name]][:限定操作符]
{
	...
}
```

操作符
- 留空 加入新节点
- @ 修改节点
- + $ 复制节点
- - ! 删除节点
- % 查询 存在时修改 不存在时创建

假如有

```
TESTNODE
{
    name = testName
    TEST = TEST_VALUE
    TEST2 = TESTVALUE2
}
```

执行

```
+TESTNODE[testName]
{
    @name = copyTestNode
    @TEST = CopyTestValue
    SUBNODE
    {
        name = subNodeName
    }
}
```

得到

```
TESTNODE
{
    name = testName
    TEST = TEST_VALUE
    TEST2 = TESTVALUE2
}

TESTNODE
{
    name = copyTestNode
    TEST = CopyTestValue
    TEST2 = TESTVALUE2
    SUBNODE
    {
        name = subNodeName
    }
}
```

值的一般形式是

```
<Op><Name-With-Wildcards>(,<index>)?
```

节点的一般形式是

```
<Op><NodeType>([<NodeNameWithWildcards>])?(:HAS[<has block>])?(,<index-or-*>)?
```




[Patch顺序](https://github.com/KSP-ModularManagement/ModuleManager/wiki/Patch-Ordering)

优先级
- :FIRST
- :BEFORE[modname]
- :FOR[modname]
- :AFTER[modname]
- :LAST[modname]
- :FINAL

Patch应用顺序

- 空操作符先运行
- 如果modname不存在 删除NEEDS, BEFORE, AFTER
- 所有:FIRST
- 没有指明优先级
- 按modname的Unicode序运行
  - :BEFORE
  - :FOR
  - :AFTER
- 所有:LAST
- 所有:FINAL

modname的有效值
通过以下操作生成不区分大小写的modname值列表 用在NEEDS BEFORE AFTER中
- 扫描所有DLL 获取modname.dll
- 所有的FOR[modname]
- 扫描GameData目录 从文件夹名称中删除空格得到modname

[手册](https://github.com/KSP-ModularManagement/ModuleManager/wiki/Module-Manager-Handbook)



## 名词暂定翻译表


Sustainer 主引擎 加速级 Booster 助推器 起飞级
kick motor 入轨引擎 kick stage kick级 入轨级
pressure-fed 挤压式 pump-fed 泵压式