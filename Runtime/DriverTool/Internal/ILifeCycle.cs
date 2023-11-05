using System;
using System.Collections.Generic;

namespace YuzeToolkit.DriverTool
{
    internal interface IUpdateCycle : IDisposable
    {
        IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper>? Wrappers { get; set; }
        int Index { get; set; }
        IUpdate Update { get; set; }
    }

    internal interface IFixedUpdateCycle : IDisposable
    {
        internal IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper>? Wrappers { get; set; }
        internal int Index { get; set; }
        IFixedUpdate FixedUpdate { get; set; }
    }

    internal interface ILateUpdateCycle : IDisposable
    {
        internal IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper>? Wrappers { get; set; }
        internal int Index { get; set; }
        ILateUpdate LateUpdate{ get; set; }
    }
}