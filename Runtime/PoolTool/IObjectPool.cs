#nullable enable
using System;

namespace YuzeToolkit.PoolTool
{
    public interface IObjectPool : IDisposable
    {
        int MaxSize { get; set; }
        int CountAll { get; }
        int CountActive { get; }
        int CountInactive { get; }
        object Get();
        IDisposable Get(out object value);
        bool Release(object value);
        void Clear();
    }
}