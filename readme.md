刚刚接触KSP的Patch翻译，有很多地方搞不明白，但为了以防忘记，还是记录一些用到的东西，来规范自己的翻译。

在Localization下按照不同Mod的路径造一个同样的路径，如果原位置有一个xx.cfg文件 翻译时就在对应位置创建一个zh-cn-MM-xx.cfg文件。
如果打算把某一个目录比如xx/下的所有文件翻译成一个patch，就起名成zh-cn.cfg。（暂定）

有些机翻保留了原文，为谨慎起见没有用变量，而是直接把英文贴在了中文后。

如果要在优先级FOR[RealismOverhaulTankTypes]前修改，就用FOR[RealismOverhaulAAATankTypes]之后就用AFTER/FOR[RealismOverhaulZZZTankTypes]。
如果要在FOR[RealismOverhaul]优先级后用FOR修改，就用FOR[RealismOverhaulXYZ]。
如果没有优先级，就也不用优先级或者用FOR[XYZLocalization]（哪个好一些？）
如果在AFTER[zPFFE]之后，就写FOR[zPFFEXYZ]。
确定没什么需要更改的，最后要覆盖的，就写:FINAL。

## 名词暂定翻译表

- Sustainer 主引擎 加速级 Booster 助推器 起飞级 （K君推荐：主引擎）
- kick motor 入轨引擎 kick stage kick级 入轨级 （K君推荐：踢进器）
- pressure-fed 挤压式 pump-fed 泵压式 （K君推荐：高压送油，泵油式）

- tank 油箱 燃料罐 储罐 罐（这个该怎么翻译！）（K君推荐：燃料罐，然后Service Module那个单独翻译成储罐）
- isogrid orthogrid gridded stringer （K君建议：三角绗架，正交绗架，绗架结构？，纵梁）
- procedural （K君建议：自定义）

- mount plate

## 语法

[教程参考](https://www.bilibili.com/opus/795218190163509272)

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


## Patch顺序

[Patch顺序](https://github.com/KSP-ModularManagement/ModuleManager/wiki/Patch-Ordering)

优先级
- :FIRST
- :BEFORE[modname]
- :FOR[modname]
- :AFTER[modname]
- :LAST[modname]
- :FINAL

Patch应用顺序

- 空操作符先运行（插入）
- 如果modname不存在 **删除NEEDS, BEFORE, AFTER**
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

## 手册

[手册](https://github.com/KSP-ModularManagement/ModuleManager/wiki/Module-Manager-Handbook)

节点 - 由尖括号保住的内容

- MODULE {}
- RESOUCE {}
- PROP {}

键值对 - 等号连接的键和值

- name = mk2LanderCabin
- maxThrust = 1500
- description = The mobile processing lab was developed to (...)

### 操作符

通用语法

- 操作符
  - 留空 创建新节点
  - `@` 编辑
  - `+`和`$`复制节点
  - `-`和`!`删除节点
  - `%`编辑、不存在时创建

- 过滤器
  - `*` 任意数量字母或数字
  - `?` 单个字符 也包括空格和特殊字符
  - `@` 包含节点
  - `-`或`!`排除节点
  - `#`包含键
  - `~`排除键
  - `:HAS[<node>]` 查找匹配过滤器的文件
  - `:NEEDS[<modname>]` 如果mod安装才生效

- 其他
  - `$`和`,` 逻辑与
  - `|` 逻辑或
  - `:Final`强制补丁最后应用

注：`HAS`不支持逻辑或 如果要写或 请写两个补丁

用例

```
// 编辑name为"SomePart"的PART
@PART[SomePart]
{
    // 修改SomePart的mass为0.625
    @mass = 0.625
    // 修改description
    @description = SomePart: now uses Xenon!

    // 修改SomePart的节点，其name为"ModuleEngines"
    @MODULE[ModuleEngines]
    {
        // 修改maxThrust为225
        @maxThrust = 2.25

        // Edit SomePart's node PROPELLANT named "LiquidFuel"
        @PROPELLANT[LiquidFuel]
        {
            // Change the PROPELLANT node name from LiquidFuel to XenonGas
            @name = XenonGas
            // Change the ratio value
            @ratio = 1.0
        }

        // 修改SomePart的节点atmosphereCurve 注意这个节点没有name
        @atmosphereCurve
        {
            // 找到"atmosphereCurve"的第一个名为"key"的键 修改
            @key,0 = 0 390
            // Edit the SECOND "key" Key from the "atmosphereCurve" property
            @key,1 = 1 320
        }

        // 移除name为"ElectricChargeOxidizer"的节点
        !PROPELLANT[Oxidizer] {}
    }

    // 在PART中创建新节点RESOURCE
    RESOURCE
    {
        // Add a name to the node RESOURCE
        name = ElectricCharge
        // Add "amount" and its value to this node
        amount = 100
        // Add "maxAmount" and its value to this node
        maxAmount = 100
    }
}
```

### 带数字的过滤器

键：
- `@example,0 = <...>` 修改**第一个**"example" 相当于`@example = <...>`
- `@example,1 = <...>`修改第二个
- 之后同理
- `@example,* = <...>`修改所有
- `@example,-1 = <...>`修改最后一个

节点 上述规则对节点也成立：
- `!EXAMPLE,0 {}` 删除第一个"EXAMPLE"节点
- `@EXAMPLE,*{...(stuff)}`修改所有
- `@MODULE[Example],*{...(stuff)}` 有名字也可以 修改所有名为"Example"的MODULE节点

### 编辑多个部件

用`*`匹配多个部件

```
@PART[B9_*]
{
    ...(stuff)
}
```

匹配所有以"B9_"开头的部件

```
@PART[B9_*]
{
    @MODULE[Module*Drill]
    {
        ...(stuff)
    }
}
```

匹配所有以"B9_"开头的部件 更新其中第一个MODULE

```
@PART[B9_*]
{
    @MODULE[Module*Drill],*
    {
        ...(stuff)
    }
}
```

匹配所有PART里所有MODULE

#### 特定节点

```
@PART[*]:HAS[@MODULE[ModuleEngines]]
{
    ...(stuff)
}
```

匹配包含MODULE.`ModuleEngines`的所有节点

```
@PART[*]:HAS[!MODULE[ModuleCommand]]
{
    ...(stuff)
}
```

这次匹配不包含的

#### 特定键

```
@PART[*]:HAS[#category[Utility]]
{
    ...(stuff)
}
```

匹配包含键值对PART.`category = Utility`的节点 是直接包含 子节点不行

```
@PART[*]:HAS[~TechRequired[]]
{
    ...(stuff)
}
```

匹配不包含任何PART.`TechRequired`键值对的节点

```
@PART[*]:HAS[#TechRequired]
{
    ...(stuff)
}
```

匹配包含键值对 值任意取

#### 特定配置

```
@PART[*]:HAS[@RESOURCE[MonoPropellant]:HAS[#maxAmount[750]]]
{
    ...(stuff)
}
```

```
@PART[*]:HAS[@MODULE[ModuleEngines]:HAS[@PROPELLANT[XenonGas]]]
{
    ...(stuff)
}
```

#### 联合搜索

```
@PART[*]:HAS[@MODULE[ModuleEngines] , @RESOURCE[SolidFuel]]
{
    ...(stuff)
}
```

```
@PART[*]:HAS[  @MODULE[ModuleEngines] :HAS [ @PROPELLANT[XenonGas] , @PROPELLANT[ElectricCharge] ]  ]
{
    ...(stuff)
}
```

#### 更深层

```
@PART[*]:HAS[!RESOURCE[ElectricCharge],@RESOURCE[*]]
{
    ...(stuff)
}
```

### 数组

```
// node_stack_top = 0.0, .9, 0.0, 0.0, 1.0, 0.0, 2
@node_stack_top[6] = 4
```
修改逗号分隔的数组的第七个数字

```
@TemperatureModifier
{
	//key = 0 150000      
	//key = 750 75000    
	@key,*[1, ] *= 2
}
```

所有名为"key"的键的第二个数字翻倍

### 一些有用的例子

```
@PART[*]:HAS[~TechRequired[]]:Final
{
    TechRequired = advScienceTech
}
```

给没有TechRequired的节点写TechRequired

```
@PART[*]:HAS[@MODULE[ModuleCommand],!MODULE[MechJebCore]]:Final
{    
    MODULE
    {
        name = MechJebCore
        MechJebLocalSettings 
        {
            MechJebModuleCustomWindowEditor {unlockTechs = flightControl}
            MechJebModuleSmartASS {unlockTechs = flightControl}
            MechJebModuleManeuverPlanner {unlockTechs = advFlightControl}
            MechJebModuleNodeEditor {unlockTechs = advFlightControl}
            MechJebModuleTranslatron {unlockTechs = advFlightControl}
            MechJebModuleWarpHelper {unlockTechs = advFlightControl}
            MechJebModuleAttitudeAdjustment {unlockTechs = advFlightControl}
            MechJebModuleThrustWindow {unlockTechs = advFlightControl}
            MechJebModuleRCSBalancerWindow {unlockTechs = advFlightControl}
            MechJebModuleRoverWindow {unlockTechs = fieldScience}
            MechJebModuleAscentGuidance {unlockTechs = unmannedTech}
            MechJebModuleLandingGuidance {unlockTechs = unmannedTech}
            MechJebModuleSpaceplaneGuidance {unlockTechs = unmannedTech}
            MechJebModuleDockingGuidance {unlockTechs = advUnmanned}
            MechJebModuleRendezvousAutopilotWindow {unlockTechs = advUnmanned}
            MechJebModuleRendezvousGuidance {unlockTechs = advUnmanned}
        }
    }
}
```

给所有pods和probes启用MechJeb

```
@EXPERIMENT_DEFINITION[*]:HAS[#id[gravityScan]]
{
    @baseValue = 5
    @scienceCap = 10
}
```

大多数示例使用PART 但是其他也可以

