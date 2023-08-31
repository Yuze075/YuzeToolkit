using System;
using System.Collections.Generic;

namespace YuzeToolkit.Utility
{
    public interface ILifeCycle : IDisposable
    {
        bool Enable { get; set; }
        IMonoBase MonoBase { get; }
    }

    internal interface IULifeCycle : ILifeCycle
    {
        IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> Wrappers { get; set; }
        int Index { get; set; }
        IUpdate Update { get; set; }
    }

    internal interface IFLifeCycle : ILifeCycle
    {
        internal IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> Wrappers { get; set; }
        internal int Index { get; set; }
        IFixedUpdate FixedUpdate { get; set; }
    }

    internal interface ILLifeCycle : ILifeCycle
    {
        internal IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> Wrappers { get; set; }
        internal int Index { get; set; }
        ILateUpdate LateUpdate{ get; set; }
    }
}