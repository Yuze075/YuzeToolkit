#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.BindableTool
{
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_BINDABLE_TOOL_USE_SHOW_VALUE
    [Serializable]
#endif
    public sealed class BindableSystem : IDisposable, IBindableRegister
    {
        ~BindableSystem() => Dispose(false);
        [NonSerialized] private bool _disposed;

#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_BINDABLE_TOOL_USE_SHOW_VALUE
        [UnityEngine.Title("总字典")] [UnityEngine.IgnoreParent] [UnityEngine.SerializeField]
        private InspectorTool.ShowDictionary<Type, object> bindables;
#else
        private readonly Dictionary<Type, object> bindables = new();
#endif

        void IBindableRegister.AddBindable<T>(T bindable)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var type = typeof(T);

            // 不存在对应的IBindable, 直接添加到列表中
            if (!bindables.TryGetValue(type, out var value))
            {
                bindables.Add(type, bindable);
                return;
            }

            // 如果已经为List<IBindable>, 则直接添加到列表中
            if (value is List<IBindable> list)
            {
                list.Add(bindable);
                return;
            }

            // 为单独的IBindable, 则创建一个新列表进行替换
            bindables[type] = new List<IBindable>
            {
                (IBindable)value,
                bindable
            };
        }

        public bool RemoveBindable<T>(T bindable) where T : IBindable
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var type = typeof(T);

            if (!bindables.TryGetValue(type, out var value)) return false;

            if (value is List<IBindable> list) return list.Remove(bindable);

            if ((IBindable)value != (IBindable)bindable) return false;

            bindables.Remove(type);
            return true;
        }


        public IEnumerable<T> GetBindables<T>() where T : IBindable
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var type = typeof(T);
            if (!bindables.TryGetValue(type, out var value)) yield break;
            if (value is List<IBindable> list)
            {
                foreach (var bindable in list) yield return (T)bindable;
                yield break;
            }

            yield return (T)value;
        }

        public bool TryGetBindables<T>([MaybeNullWhen(false)] out T[] bindables) where T : IBindable
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var type = typeof(T);
            if (!this.bindables.TryGetValue(type, out var value))
            {
                bindables = null;
                return false;
            }

            if (value is List<IBindable> list)
            {
                bindables = new T[list.Count];
                var count = list.Count;
                for (var index = 0; index < count; index++) bindables[index] = (T)list[index];
                return true;
            }

            bindables = new T[1];
            bindables[0] = (T)value;
            return true;
        }

        /// <summary>
        /// 获取<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public T? GetBindable<T>() where T : IBindable
        {
            var type = typeof(T);
            if (!bindables.TryGetValue(type, out var value)) return default;

            if (value is List<IBindable> list) return list.Count == 0 ? default : (T)list[0];
            return (T)value;
        }

        /// <summary>
        /// 尝试获取到<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public bool TryGetBindable<T>([MaybeNullWhen(false)] out T t) where T : IBindable
        {
            var type = typeof(T);
            if (!bindables.TryGetValue(type, out var value))
            {
                t = default;
                return false;
            }

            if (value is List<IBindable> list)
            {
                if (list.Count == 0)
                {
                    t = default;
                    return false;
                }

                t = (T)list[0];
                return true;
            }

            t = (T)value;
            return true;
        }

        /// <summary>
        /// 是否存在<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public bool ContainsBindable<T>() where T : IBindable => bindables.ContainsKey(typeof(T));

        #region IDisposable

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_disposed) return;
            if (isDisposing)
                foreach (var value in bindables.Values)
                {
                    if (value is List<IBindable> bindables)
                    {
                        var count = bindables.Count;
                        for (var index = 0; index < count; index++) bindables[index].Dispose();
                        bindables.Clear();
                    }

                    ((IBindable)value).Dispose();
                }

            bindables.Clear();
            _disposed = true;
        }

        #endregion
    }
}