# Behavior-Tree-Editor
基于Unity3D的行为树编辑器，保存时导出xml和lua文件。

# 目录结构
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
    -	客户端AI目录：Assets\..\Lua\Data\ServerData\AIClient
    - 服务端AI目录：Assets\..\Lua\Data\ServerData\AIScript

# 关键功能
## 获取所有AI节点类型
1.	所有AI节点均定义在指定命名空间下。
2.	遍历当前应用程序域下的所有程序集，查找符合条件的类类型。
3.	使用字典存储。
4.	入口：BTEditorManager.RefreshNodeTypes。
## 数据的序列化和反序列化
1.	每颗行为树隶属于一个BTAsset，即行为树编辑文件。
2.	行为树数据以xml格式保存在BTAsset中。
3.	Xml数据的处理使用C#的内置xml库。
4.	在编辑器窗口的内容有变动时，刷新行为树数据。
5.	针对每个节点中数据的设置和读取，采用C#的反射机制以简化逻辑。
## 存储格式转换
1.	工程开发使用lua脚本，需要将xml格式转换为可用的lua格式；
2.	解析xml数据使用C#的内置库；
3.	在AINode中，将xml数据解析为以顺序数字为key的哈希table。
## 属性
1.	满足的需求
    - 自定义AI节点的菜单项显示路径。
    - 自定义AI节点对应的图标。
    - 标识需要自动生成值的字段。
2.	使用C#的反射机制获取节点或字段包含的属性。
3.	遍历属性，根据需求进行相应处理。
## BaseRoot和Root
1.	BaseRoot为当前行为树的最根节点，包含
  - 以Root为起始主AI树。
  - 不以Root为起始的条件、新号、参数列表树。
2.	Root用于遍历时识别主AI树的起始节点。
