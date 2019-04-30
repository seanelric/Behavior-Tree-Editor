# Behavior-Tree-Editor
基于Unity3D的行为树编辑器，保存时导出xml和lua文件。

## Description
- Author
    - Sean.Yu
- Supported
    - 画布网格绘制
    - 节点显示指定图片
    - 拖动画布
    - 拖动节点
    - 滚动条
    - 右键菜单操作
    - 添加、删除节点
    - 节点间连线
    - 缩放
    - 复制、粘贴

## 操作方式
1. 创建
    - Project下右键单击目录，选择Create\Behavior Tree
2. 单选
    - 点击节点
3. 多选
    - 鼠标框选：Alt + 按住鼠标左键画框选取（点中画布）
    - 点击复选；Ctrl + 鼠标左键点击节点
4. 拖拽
    - 拖拽单个节点：按住鼠标左键拖拽（点中节点）
    - 拖拽节点及其子节点：Shift + 拖拽单节点操作
5. 移动可视区域
    - 鼠标左键拖拽背景
6. 弹出菜单
    - 鼠标右键
7. 滑动垂直滚动条
    - 滚轮
8. 缩放画布
    - Alt + 滚轮
9. 复制
    - 对已选中节点使用菜单Copy选项 或 “Ctrl + C”组合键
10. 粘贴
    - 复制后，在背景上右键菜单使用Paste选项 或 “Ctrl + V”组合键
11. 添加
    - 菜单Add选项（点中画布）
12. 删除
    - 对已选中节点使用菜单Delete选项或“Del”按键

## 目录结构
1. 编辑工具目录
    - 根目录：Assets\EditTools\BehaviorTree
    - 编辑器脚本目录：Assets\EditTools\BehaviorTree \Editor
    - 行为树节点目录：Assets\EditTools\BehaviorTree\Nodes
    - 显示用资源目录：Assets\EditTools\BehaviorTree\Resources
    - 调试器框架目录：Assets\EditTools\BehaviorTree\Debug
2. 数据目录
    - 根目录：Assets\EditTools\Data\BehaviorTree
    - 行为树编辑文件目录：Assets\EditTools\Data\BehaviorTree\Asset
    - 保存时导出的xml文件目录：Assets\EditTools\Data\BehaviorTree\Xml
    - 保存时到处的Lua文件目录：Assets\EditTools\Data\BehaviorTree\Lua

## 关键功能
### 获取所有AI节点类型
1.	所有AI节点均定义在指定命名空间下。
2.	遍历当前应用程序域下的所有程序集，查找符合条件的类类型。
3.	使用字典存储。
4.	入口：BTEditorManager.RefreshNodeTypes。
### 数据的序列化和反序列化
1.	每颗行为树隶属于一个BTAsset，即行为树编辑文件。
2.	行为树数据以xml格式保存在BTAsset中。
3.	Xml数据的处理使用C#的内置xml库。
4.	在编辑器窗口的内容有变动时，刷新行为树数据。
5.	针对每个节点中数据的设置和读取，采用C#的反射机制以简化逻辑。
### 存储格式转换
1.	工程开发使用lua脚本，需要将xml格式转换为可用的lua格式；
2.	解析xml数据使用C#的内置库；
3.	在AINode中，将xml数据解析为以顺序数字为key的哈希table。
### 属性
1.	满足的需求
    - 自定义AI节点的菜单项显示路径。
    - 自定义AI节点对应的图标。
    - 标识需要自动生成值的字段。
2.	使用C#的反射机制获取节点或字段包含的属性。
3.	遍历属性，根据需求进行相应处理。
### BaseRoot和Root
1.	BaseRoot为当前行为树的最根节点，包含
  - 以Root为起始主AI树。
  - 不以Root为起始的条件、新号、参数列表树。
2.	Root用于遍历时识别主AI树的起始节点。
