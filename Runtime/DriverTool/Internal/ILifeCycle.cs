using System;
using System.Collections.Generic;

namespace YuzeToolkit.DriverTool
{
    internal interface IUpdateCycle : IDisposable
    {
        IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper>? Wrappers { get; set; }
        int Index { get; set; }
        IUpdate Update { get; }
    }

    internal interface IFixedUpdateCycle : IDisposable
    {
        IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper>? Wrappers { get; set; }
        int Index { get; set; }
        IFixedUpdate FixedUpdate { get; }
    }

    internal interface ILateUpdateCycle : IDisposable
    {
        IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper>? Wrappers { get; set; }
        int Index { get; set; }
        ILateUpdate LateUpdate { get; }
    }
}