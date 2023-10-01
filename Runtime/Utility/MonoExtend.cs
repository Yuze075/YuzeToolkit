using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    // public sealed class IdMono : TagMonoBase
    // {
    //     public override string Info => "用于处理代码脚本绑定相关逻辑, 快速进行脚本索引";
    //     internal string Id => id;
    //     [SerializeField] private string id;
    //     [SerializeField] private bool autoGetComponents = true;
    //     [SerializeField] private Component[] bindComponents;
    //
    //     private bool _isGetComponents;
    //     protected override void Awake()
    //     {
    //         base.Awake();
    //         AutoGetComponents();
    //     }
    //
    //     private void AutoGetComponents()
    //     {
    //         if (!autoGetComponents || _isGetComponents) return;
    //         
    //         _isGetComponents = true;
    //         bindComponents = GetComponents<Component>();
    //     }
    //
    //     public T GetComponentById<T>()
    //     {
    //         AutoGetComponents();
    //         return bindComponents.OfType<T>().FirstOrDefault();
    //     }
    //
    //     public bool TryGetComponentById<T>(out T t)
    //     {
    //         AutoGetComponents();
    //         t = bindComponents.OfType<T>().FirstOrDefault();
    //         return t != null;
    //     }
    //
    //     public T[] GetComponentsById<T>()
    //     {
    //         AutoGetComponents();
    //         return bindComponents.OfType<T>().ToArray();
    //     }
    // }
    
    // public static class MonoExtend
    // {
    //     // private static bool TryGetIdMonoBase(this GameObject gameObject, string id, out IdMonoBase t)
    //     // {
    //     //     t = gameObject.GetComponents<IdMonoBase>().FirstOrDefault(idMonoBase => idMonoBase.Id == id);
    //     //     return t != null;
    //     // }
    //     //
    //     // private static bool TryGetIdMonoBaseInChildren(this GameObject gameObject, string id, out IdMonoBase t)
    //     // {
    //     //     t = gameObject.GetComponentsInChildren<IdMonoBase>().FirstOrDefault(idMonoBase => idMonoBase.Id == id);
    //     //     return t != null;
    //     // }
    //     //
    //     // private static bool TryGetIdMonoBaseInParent(this GameObject gameObject, string id, out IdMonoBase t)
    //     // {
    //     //     t = gameObject.GetComponentsInParent<IdMonoBase>().FirstOrDefault(idMonoBase => idMonoBase.Id == id);
    //     //     return t != null;
    //     // }
    //
    //
    //     public class GetWrapper
    //     {
    //         public GetWrapper(GameObject gameObject) => _gameObject = gameObject;
    //         private readonly EGetType[] _getTypes = { EGetType.Null, EGetType.Null, EGetType.Null };
    //         private readonly EByType[] _byTypes = { EByType.Null, EByType.Null };
    //         private readonly GameObject _gameObject;
    //         private string _id;
    //         private bool _includeInactive;
    //
    //         /// <summary>
    //         /// 从自身<see cref="GameObject"/>中获取
    //         /// </summary>
    //         public GetWrapper InSelf() => AddGetType(EGetType.Self);
    //
    //         /// <summary>
    //         /// 从子物体的<see cref="GameObject"/>中获取
    //         /// </summary>
    //         public GetWrapper InChildren() => AddGetType(EGetType.Children);
    //
    //         /// <summary>
    //         /// 从父物体的<see cref="GameObject"/>中获取
    //         /// </summary>
    //         public GetWrapper InParent() => AddGetType(EGetType.Parent);
    //
    //         /// <summary>
    //         /// 使用Unity默认方法获取
    //         /// </summary>
    //         public GetWrapper ByDefault() => AddGetType(EByType.Default);
    //
    //         /// <summary>
    //         /// 使用<see cref="id"/>找到对应<see cref="IdMono"/>进行获取
    //         /// </summary>
    //         /// <param name="id"></param>
    //         /// <returns></returns>
    //         public GetWrapper ById(string id)
    //         {
    //             _id = id;
    //             return AddGetType(EByType.Id);
    //         }
    //
    //         /// <summary>
    //         /// 是否查找失活的对象
    //         /// </summary>
    //         public GetWrapper IncludeInactive(bool includeInactive = true)
    //         {
    //             _includeInactive = includeInactive;
    //             return this;
    //         }
    //
    //         /// <summary>
    //         /// Out返回对象
    //         /// </summary>
    //         /// <returns>是否成功获取到对象</returns>
    //         public bool Out<T>(out T value)
    //         {
    //             if (_byTypes[0] == EByType.Null)
    //             {
    //                 _byTypes[0] = EByType.Default;
    //             }
    //
    //             for (var index = 0; index < 2; index++)
    //             {
    //                 var byType = _byTypes[index];
    //                 switch (byType)
    //                 {
    //                     case EByType.Default:
    //                         if (TryGetByGetTypes(out value)) return true;
    //                         break;
    //                     case EByType.Id:
    //                         if (TryGetByGetTypesAndId(out value)) return true;
    //                         break;
    //                     case EByType.Null:
    //                         break;
    //                     default:
    //                         throw new ArgumentOutOfRangeException();
    //                 }
    //             }
    //
    //             value = default;
    //             return false;
    //         }
    //
    //         /// <summary>
    //         /// 直接返回对象
    //         /// </summary>
    //         public T Get<T>() => Out(out T value) ? value : default;
    //
    //         /// <summary>
    //         /// 返回查找对象, 如果没有就添加一个对象在当前<see cref="GameObject"/>上
    //         /// </summary>
    //         public T GetOrAdd<T>() where T : Component => Out(out T value) ? value : _gameObject.AddComponent<T>();
    //
    //         /// <summary>
    //         /// 返回一个目标对象的List
    //         /// </summary>
    //         public List<T> GetList<T>()
    //         {
    //             var list = new List<T>();
    //             if (_byTypes[0] == EByType.Null)
    //             {
    //                 _byTypes[0] = EByType.Default;
    //             }
    //
    //             for (var index = 0; index < 2; index++)
    //             {
    //                 var byType = _byTypes[index];
    //                 switch (byType)
    //                 {
    //                     case EByType.Default:
    //                         list.AddRange(GetByGetTypes<T>());
    //                         break;
    //                     case EByType.Id:
    //                         list.AddRange(GetByGetTypesAndId<T>());
    //                         break;
    //                     case EByType.Null:
    //                         break;
    //                     default:
    //                         throw new ArgumentOutOfRangeException();
    //                 }
    //             }
    //
    //             return list;
    //         }
    //
    //         /// <summary>
    //         /// Out返回对象
    //         /// </summary>
    //         /// <param name="includeInactive">是否查找失活的对象</param>
    //         /// <param name="value"></param>
    //         /// <returns>是否成功获取到对象</returns>
    //         public bool Out<T>(bool includeInactive, out T value) => IncludeInactive(includeInactive).Out(out value);
    //
    //         /// <summary>
    //         /// 直接返回对象
    //         /// </summary>
    //         /// <param name="includeInactive">是否查找失活的对象</param>
    //         public T Get<T>(bool includeInactive) => IncludeInactive(includeInactive).Get<T>();
    //
    //         /// <summary>
    //         /// 返回查找对象, 如果没有就添加一个对象在当前<see cref="GameObject"/>上
    //         /// </summary>
    //         /// <param name="includeInactive">是否查找失活的对象</param>
    //         public T GetOrAdd<T>(bool includeInactive) where T : Component =>
    //             Out(includeInactive, out T value) ? value : _gameObject.AddComponent<T>();
    //
    //         /// <summary>
    //         /// 返回一个目标对象的List
    //         /// </summary>
    //         /// <param name="includeInactive">是否查找失活的对象</param>
    //         public List<T> GetList<T>(bool includeInactive) => IncludeInactive(includeInactive).GetList<T>();
    //
    //
    //         #region MyRegion
    //
    //         private bool TryGetByGetTypes<T>(out T value)
    //         {
    //             if (_getTypes[0] == EGetType.Null)
    //             {
    //                 _getTypes[0] = EGetType.Self;
    //             }
    //
    //             for (var index = 0; index < 3; index++)
    //             {
    //                 var getType = _getTypes[index];
    //
    //                 switch (getType)
    //                 {
    //                     case EGetType.Self:
    //                         if (TryGetInSelf(out value)) return true;
    //                         break;
    //                     case EGetType.Children:
    //                         if (TryGetInChildren(out value)) return true;
    //                         break;
    //                     case EGetType.Parent:
    //                         if (TryGetInParent(out value)) return true;
    //                         break;
    //                     case EGetType.Null:
    //                         break;
    //                     default:
    //                         throw new ArgumentOutOfRangeException();
    //                 }
    //             }
    //
    //             value = default;
    //             return false;
    //         }
    //
    //         private bool TryGetByGetTypesAndId<T>(out T value)
    //         {
    //             if (_getTypes[0] == EGetType.Null)
    //             {
    //                 _getTypes[0] = EGetType.Self;
    //             }
    //
    //             for (var index = 0; index < 3; index++)
    //             {
    //                 var getType = _getTypes[index];
    //
    //                 switch (getType)
    //                 {
    //                     case EGetType.Self:
    //                     {
    //                         if (TryGetIdMonoBase(out var idMono) && idMono.TryGetComponentById(out value)) return true;
    //                         break;
    //                     }
    //                     case EGetType.Children:
    //                     {
    //                         if (TryGetIdMonoBaseInChildren(out var idMono) && idMono.TryGetComponentById(out value))
    //                             return true;
    //                         break;
    //                     }
    //                     case EGetType.Parent:
    //                     {
    //                         if (TryGetIdMonoBaseInParent(out var idMono) && idMono.TryGetComponentById(out value))
    //                             return true;
    //                         break;
    //                     }
    //                     case EGetType.Null:
    //                         break;
    //                     default:
    //                         throw new ArgumentOutOfRangeException();
    //                 }
    //             }
    //
    //             value = default;
    //             return false;
    //         }
    //
    //         private IEnumerable<T> GetByGetTypes<T>()
    //         {
    //             if (_getTypes[0] == EGetType.Null)
    //             {
    //                 _getTypes[0] = EGetType.Self;
    //             }
    //
    //             for (var index = 0; index < 3; index++)
    //             {
    //                 var getType = _getTypes[index];
    //
    //                 switch (getType)
    //                 {
    //                     case EGetType.Self:
    //                         foreach (var t in GetInSelf<T>()) yield return t;
    //                         break;
    //                     case EGetType.Children:
    //                         foreach (var t in GetInChildren<T>()) yield return t;
    //                         break;
    //                     case EGetType.Parent:
    //                         foreach (var t in GetInParent<T>()) yield return t;
    //                         break;
    //                     case EGetType.Null:
    //                         break;
    //                     default:
    //                         throw new ArgumentOutOfRangeException();
    //                 }
    //             }
    //         }
    //
    //         private IEnumerable<T> GetByGetTypesAndId<T>()
    //         {
    //             if (_getTypes[0] == EGetType.Null)
    //             {
    //                 _getTypes[0] = EGetType.Self;
    //             }
    //
    //             for (var index = 0; index < 3; index++)
    //             {
    //                 var getType = _getTypes[index];
    //                 switch (getType)
    //                 {
    //                     case EGetType.Self:
    //                     {
    //                         if (TryGetIdMonoBase(out var idMono))
    //                             foreach (var t in idMono.GetComponentsById<T>())
    //                                 yield return t;
    //                         break;
    //                     }
    //                     case EGetType.Children:
    //                     {
    //                         if (TryGetIdMonoBaseInChildren(out var idMono))
    //                             foreach (var t in idMono.GetComponentsById<T>())
    //                                 yield return t;
    //                         break;
    //                     }
    //                     case EGetType.Parent:
    //                     {
    //                         if (TryGetIdMonoBaseInParent(out var idMono))
    //                             foreach (var t in idMono.GetComponentsById<T>())
    //                                 yield return t;
    //                         break;
    //                     }
    //                     case EGetType.Null:
    //                         break;
    //                     default:
    //                         throw new ArgumentOutOfRangeException();
    //                 }
    //             }
    //         }
    //
    //         #endregion
    //
    //         #region MyRegion
    //
    //         private bool TryGetInSelf<T>(out T value) => _gameObject.TryGetComponent(out value);
    //
    //         private bool TryGetInChildren<T>(out T value)
    //         {
    //             value = _gameObject.GetComponentInChildren<T>(_includeInactive);
    //             return value != null;
    //         }
    //
    //         private bool TryGetInParent<T>(out T value)
    //         {
    //             value = _gameObject.GetComponentInParent<T>(_includeInactive);
    //             return value != null;
    //         }
    //
    //         private IEnumerable<T> GetInSelf<T>() => _gameObject.GetComponents<T>();
    //         private IEnumerable<T> GetInChildren<T>() => _gameObject.GetComponentsInChildren<T>(_includeInactive);
    //         private IEnumerable<T> GetInParent<T>() => _gameObject.GetComponentsInParent<T>(_includeInactive);
    //
    //         private bool TryGetIdMonoBase(out IdMono idMono)
    //         {
    //             idMono = _gameObject.GetComponents<IdMono>().FirstOrDefault(idMonoBase => idMonoBase.Id == _id);
    //             return idMono != null;
    //         }
    //
    //         private bool TryGetIdMonoBaseInChildren(out IdMono idMono)
    //         {
    //             idMono = _gameObject.GetComponentsInChildren<IdMono>(_includeInactive)
    //                 .FirstOrDefault(idMonoBase => idMonoBase.Id == _id);
    //             return idMono != null;
    //         }
    //
    //         private bool TryGetIdMonoBaseInParent(out IdMono idMono)
    //         {
    //             idMono = _gameObject.GetComponentsInParent<IdMono>(_includeInactive)
    //                 .FirstOrDefault(idMonoBase => idMonoBase.Id == _id);
    //             return idMono != null;
    //         }
    //
    //         #endregion
    //
    //         private GetWrapper AddGetType(EGetType getType)
    //         {
    //             for (var index = 0; index < 3; index++)
    //             {
    //                 var type = _getTypes[index];
    //
    //                 // 存在相同的返回
    //                 if (type == getType) return this;
    //
    //                 // 不为空继续
    //                 if (type != EGetType.Null) continue;
    //
    //                 _getTypes[index] = getType;
    //                 return this;
    //             }
    //
    //             return this;
    //         }
    //
    //         private GetWrapper AddGetType(EByType byType)
    //         {
    //             for (var index = 0; index < 2; index++)
    //             {
    //                 var type = _byTypes[index];
    //
    //                 // 存在相同的返回
    //                 if (type == byType) return this;
    //
    //                 // 不为空继续
    //                 if (type != EByType.Null) continue;
    //
    //                 _byTypes[index] = byType;
    //                 return this;
    //             }
    //
    //             return this;
    //         }
    //
    //         private enum EGetType
    //         {
    //             Null,
    //             Self,
    //             Children,
    //             Parent,
    //         }
    //
    //         private enum EByType
    //         {
    //             Null,
    //             Default,
    //             Id,
    //         }
    //     }
    //
    //     /// <summary>
    //     /// 用于快速获取<see cref="UnityEngine.Component"/>
    //     /// </summary>
    //     public static GetWrapper Component(this GameObject gameObject)
    //     {
    //         return new GetWrapper(gameObject);
    //     }
    //
    //     /// <summary>
    //     /// 用于快速获取<see cref="UnityEngine.Component"/>
    //     /// </summary>
    //     public static GetWrapper Component(this Component monoBehaviour)
    //     {
    //         return new GetWrapper(monoBehaviour.gameObject);
    //     }
    //
    //     // public static T GetComponentById<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBase(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return null;
    //     // }
    //     //
    //     // public static bool TryGetComponentById<T>(this GameObject gameObject, string id, out T t) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBase(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = null;
    //     //     return false;
    //     // }
    //     //
    //     // public static T[] GetComponentsById<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBase(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentsById<T>();
    //     //     }
    //     //
    //     //     return Array.Empty<T>();
    //     // }
    //     //
    //     // public static T GetComponentByIdOrDefault<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBase(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return gameObject.GetComponent<T>();
    //     // }
    //     //
    //     // public static bool TryGetComponentByIdOrDefault<T>(this GameObject gameObject, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBase(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     return gameObject.TryGetComponent(out t);
    //     // }
    //     //
    //     // public static T GetComponentInChildrenById<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInChildren(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return null;
    //     // }
    //     //
    //     // public static bool TryGetComponentInChildrenById<T>(this GameObject gameObject, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInChildren(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = null;
    //     //     return false;
    //     // }
    //     //
    //     // public static T[] GetComponentsInChildrenById<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInChildren(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentsById<T>();
    //     //     }
    //     //
    //     //     return Array.Empty<T>();
    //     // }
    //     //
    //     // public static T GetComponentInChildrenByIdOrDefault<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInChildren(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return gameObject.GetComponentInChildren<T>();
    //     // }
    //     //
    //     // public static bool TryGetComponentInChildrenByIdOrDefault<T>(this GameObject gameObject, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInChildren(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = gameObject.GetComponentInChildren<T>();
    //     //     return t != null;
    //     // }
    //     //
    //     // public static T GetComponentInParentById<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInParent(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return null;
    //     // }
    //     //
    //     // public static bool TryGetComponentInParentById<T>(this GameObject gameObject, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInParent(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = null;
    //     //     return false;
    //     // }
    //     //
    //     // public static T[] GetComponentsInParentById<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInParent(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentsById<T>();
    //     //     }
    //     //
    //     //     return Array.Empty<T>();
    //     // }
    //     //
    //     // public static T GetComponentInParentByIdOrDefault<T>(this GameObject gameObject, string id) where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInParent(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return gameObject.GetComponentInParent<T>();
    //     // }
    //     //
    //     // public static bool TryGetComponentInParentByIdOrDefault<T>(this GameObject gameObject, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (gameObject.TryGetIdMonoBaseInParent(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = gameObject.GetComponentInParent<T>();
    //     //     return t != null;
    //     // }
    //     //
    //     // private static bool TryGetIdMonoBase(this Component component, string id, out IdMonoBase t)
    //     // {
    //     //     t = component.GetComponents<IdMonoBase>().FirstOrDefault(idMonoBase => idMonoBase.Id == id);
    //     //     return t != null;
    //     // }
    //     //
    //     // private static bool TryGetIdMonoBaseInChildren(this Component component, string id, out IdMonoBase t)
    //     // {
    //     //     t = component.GetComponentsInChildren<IdMonoBase>().FirstOrDefault(idMonoBase => idMonoBase.Id == id);
    //     //     return t != null;
    //     // }
    //     //
    //     // private static bool TryGetIdMonoBaseInParent(this Component component, string id, out IdMonoBase t)
    //     // {
    //     //     t = component.GetComponentsInParent<IdMonoBase>().FirstOrDefault(idMonoBase => idMonoBase.Id == id);
    //     //     return t != null;
    //     // }
    //     //
    //     // public static T GetComponentById<T>(this MonoBehaviour component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBase(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return null;
    //     // }
    //     //
    //     // public static bool TryGetComponentById<T>(this Component component, string id, out T t) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBase(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = null;
    //     //     return false;
    //     // }
    //     //
    //     // public static T[] GetComponentsById<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBase(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentsById<T>();
    //     //     }
    //     //
    //     //     return Array.Empty<T>();
    //     // }
    //     //
    //     // public static T GetComponentByIdOrDefault<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBase(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return component.GetComponent<T>();
    //     // }
    //     //
    //     // public static bool TryGetComponentByIdOrDefault<T>(this Component component, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBase(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     return component.TryGetComponent(out t);
    //     // }
    //     //
    //     // public static T GetComponentInChildrenById<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInChildren(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return null;
    //     // }
    //     //
    //     // public static bool TryGetComponentInChildrenById<T>(this Component component, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInChildren(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = null;
    //     //     return false;
    //     // }
    //     //
    //     // public static T[] GetComponentsInChildrenById<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInChildren(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentsById<T>();
    //     //     }
    //     //
    //     //     return Array.Empty<T>();
    //     // }
    //     //
    //     // public static T GetComponentInChildrenByIdOrDefault<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInChildren(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return component.GetComponentInChildren<T>();
    //     // }
    //     //
    //     // public static bool TryGetComponentInChildrenByIdOrDefault<T>(this Component component, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInChildren(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = component.GetComponentInChildren<T>();
    //     //     return t != null;
    //     // }
    //     //
    //     // public static T GetComponentInParentById<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInParent(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return null;
    //     // }
    //     //
    //     // public static bool TryGetComponentInParentById<T>(this Component component, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInParent(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = null;
    //     //     return false;
    //     // }
    //     //
    //     // public static T[] GetComponentsInParentById<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInParent(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentsById<T>();
    //     //     }
    //     //
    //     //     return Array.Empty<T>();
    //     // }
    //     //
    //     // public static T GetComponentInParentByIdOrDefault<T>(this Component component, string id) where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInParent(id, out var idMonoBase))
    //     //     {
    //     //         return idMonoBase.GetComponentById<T>();
    //     //     }
    //     //
    //     //     return component.GetComponentInParent<T>();
    //     // }
    //     //
    //     // public static bool TryGetComponentInParentByIdOrDefault<T>(this Component component, string id, out T t)
    //     //     where T : class
    //     // {
    //     //     if (component.TryGetIdMonoBaseInParent(id, out var idMonoBase) &&
    //     //         idMonoBase.TryGetComponentById(out t))
    //     //     {
    //     //         return true;
    //     //     }
    //     //
    //     //     t = component.GetComponentInParent<T>();
    //     //     return t != null;
    //     // }
    // }
}