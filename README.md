# YuzeToolkit

* 一个基于接口思想实现的工具库, 用于辅助Unity游戏开发

## Utility

* 通用的工具类, 包含了公用的一些基础工具

### 1. `FGUI`

* FGUI
  * 支持快速对`Rect`进行矩形处理
* FGUILayout
  * 支持快速进行`Horizontal`和`Vertical`的分组管理
  * 支持快速绘制`Button`
  * 支付快速绘制`Field`
      ```csharp
    var listStr = new List<string?>();
    var listInt = new List<int>();
    // 绘制水平分组
    using (FGUILayout.Horizontal)
    {
        // 绘制退出按钮
        FGUILayout.Button(Application.Quit, "Quit");
  
        // 绘制垂直分组
        using (FGUILayout.Vertical)
        {
            for (var i = 0; i < listStr.Count; i++)
            {
                var index = i;
                // 绘制字段
                FGUILayout.Field(
                    () => listStr[index], 
                    value => listStr[index] = value, 
                    $"Str{index}");
            }
        }
  
        // 绘制垂直分组
        using (FGUILayout.Vertical)
        {
           foreach (var i in listInt)
                // 绘制字段
                FGUILayout.Field(i, $"Int_{i}");
        }
    }
      ```


### 2. `IDisposableNode`
* 使用`DisposableList`对`IDisposable`进行管理
* 支持`UnRegister`的列表引用释放方法
* 支持`IDisposeNode`的附加式`IDisposable`管理

### 3. `IObjectPool`
* 对于csharp的对象池管理
  * `GenericPoolBase`: 泛型对象池的静态基类
  * `GenericPool`: 支持带有`new()`方法的对象的对象池缓存
  * `CollectionPool`: 支持带有`new()`方法的`ICollection<TItem>`对象的对象池缓存
* `IObjectPool`和`ObjectPool`的默认对象池实现

### 4. `ReflectionHelper`
* 反射帮助类, 可以快速获取需要的反射对象

### 5. `SerializeType`
* 可以序列化的Type, 需要`SerializeType`(或者`String`)配合`TypeSelectorAttribute`一起使用

### 6. `USerialize`
* 可以序列化的`UnityObject`的接口类型(只能序列化Prefab)
  * 例如: 我需要序列化一个`IReadOnlyList<string>`的`UnityObject`, 可以使用`USerialize<IReadOnlyList<string>>`来实现

## DriverTool

* 封装的更新管理器, 用于管理`Update` `FixeUpdate` `LateUpdate`的更新逻辑

## LogTool

* 日志工具, 提供了日志的输出方案

## BindableTool

* 可绑定的变量, 用于解决UI绑定数据问题, 或者回调式数据问题

## EventTool

* 事件工具, 提供方便快捷的事件解决方案

## InspectorTool

* 拓展Inspector面板的显示功能, 添加`Show` `ShowList` `ShowDictionary`等显示类型

## IocTool -> InspectorTool

* 依赖注入工具, 提供了依赖注入的方案

## UITool -> IocTool

* 基于MVC的简易UI框架

## DataTool -> InspectorTool, IocTool

* 基于`IocTool`, 处理游戏Model数据管理






