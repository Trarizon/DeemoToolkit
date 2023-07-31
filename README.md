# DeemoToolkit

DEEMO相关的一些工具

## Commands

命令行工具，如果不是用命令行过于复杂的都扔在这里了

### ChartPainting
```
paint <file...> [-l:str?] [-r:str?] [-v:int] [-m] [-g:int] [-s:float] [-t:int] [-i:float] [-o:float] [--skip] [--lp:int] [--tp:int] [--rp:int] [--bp:int]
```
将谱面绘制为png图片

参数|默认值|注释
:-:|:-:|:--
`-l`|`null`|左侧标记，格式为`combo:<int> | time:<float>`可设置按固定combo或时间值显示标记，格式错误则默认无显示
`-r`|`null`|右侧标记，同时会在谱面中显示横向格线
`-v`|`0`|游戏版本(0:Deemo, 1:DeemoReborn, 2:DeemoII)，目前实现了Deemo和DeemoII的显示，DeemoII资源缺失
`-m`|`false`|是否在背景显示midi，**未实现**
`-g`|`0`|两段谱面之间的间隔
`-s`|`1`|谱面速度，即note之间的纵向间隔
`-t`|`20`|谱面分段的基准时间，单位为秒
`-i`|`0`|谱面下方过渡区域长度，会显示部分前段谱面以衔接，单位为秒
`-o`|`0`|谱面上方过渡区域长度，单位为秒
`--skip`|`false`|跳过谱面开头的空白部分
`--lp`|`0`|图像左边空白部分，单位为像素
`--tp`|`0`|图像上方空白部分，单位为像素
`--rp`|`0`|图像右边空白部分，单位为像素
`--bp`|`0`|图像下方空白部分，单位为像素


### RearrangeByPitch
```
mid <file...> [-f]
```
将所有钢琴音转换成一个宽度为0.2的note，并按音高排列

参数|默认值|注释
:-:|:-:|:--
`-f`|`false`|是否固定音高映射到pos的边界，固定情况下pos -2/+2对应音高的21/108

### RandomNotes
```
rnd <file...> [-s:int?] [-w] [-t:float] [-x:float]
```
将所有可视note的位置（以及大小）随机排列，
会避免click双押/伪双距离过近的问题，以及由于算法问题，多押可能会无法生成
（默认情况下，四押及以上有可能会无法生成）

参数|默认值|注释
:-:|:-:|:--
`-s`|`null`|随机数种子，不指定则使用`Random`的无参构造
`-w`|`false`|是否对note的大小进行随机，默认不随机
`-t`|`0.417`|视为伪双的时间界限，默认为bpm240的24分音符
`-x`|`1`|双押/伪双的横向距离界限，双押的距离不会小于这个值

### ToAllClickChart
```
black <file...>
```
将所有note转为click

## InfoFileGenerator

用于生成DeemoDIY和DemooPlayer所用的ini和txt文件，
附带对文件的重命名工具

<details>
<summary>详细说明</summary>

填入对应信息后点击**Export INI**和**Export TXT**可以在导出对应的文件到Path文本框所在位置

点击**Export As Configuration**将目前填写的信息作为默认信息导出至`Configuration/InfoFileGenerator.json`
（*最有用的就是设定谱师名.jpg*

点击**Txt Settings**展开Demooplayer相关的详细的设置

可以将文件夹拖入到**Path**文本框或左下角空白框，会将该文件夹作为输出路径。
且会识别文件夹内部分文件，点击**Normalize**会按照DEEMO DIY的导入需求重命名文件

可识别文件有：
- `*.mp3` 识别为音乐或预览音频文件，若有两个mp3则较长的被认为是音乐文件，分别重命名为`Music.mp3`和`Preview.mp3`
- `*.easy|normal|hard|extra|ultra.json` 单词首字母可以大写，识别为谱面文件，`ultra`为了只是兼容可能的旧版diy，最好没用（，重命名为`Easy|Normal|Hard|Extra.json`
- `*.png` 识别为Cover，重命名为`Cover.png`，若没有png文件则会生成一个0kb的`Cover.png`（diy里能跑就是了

如果是dnt写的谱的话，导出的谱面文件就符合这个格式，扔文件夹里可以直接重命名


</details>
