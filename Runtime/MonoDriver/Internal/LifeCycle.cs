using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.Utility
{
    internal abstract class LifeCycleBase : IDisposable
    {
        public void Dispose()
        {
            if (this is IUpdateCycle uLifeCycle) OnDispose(uLifeCycle);
            if (this is IFixedUpdateCycle fLifeCycle) OnDispose(fLifeCycle);
            if (this is ILateUpdateCycle lLifeCycle) OnDispose(lLifeCycle);
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public static IDisposable Build(IMonoBase monoBase)
        {
            return monoBase switch
            {
                IUpdate and IFixedUpdate and ILateUpdate => new UFLCycle(monoBase),
                IUpdate and IFixedUpdate => new UFCycle(monoBase),
                IUpdate and ILateUpdate => new ULCycle(monoBase),
                IFixedUpdate and ILateUpdate => new FLCycle(monoBase),
                IUpdate => new UCycle(monoBase),
                IFixedUpdate => new FCycle(monoBase),
                ILateUpdate => new LCycle(monoBase),
                _ => new NullDisposable()
            };
        }

        #region LifeCycle

        private static void OnDispose(IUpdateCycle updateCycle)
        {
            var index = updateCycle.Index;
            if (index == -1) return;

            var list = updateCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Dispose(index);
            list[index] = wrapper;

            updateCycle.Wrappers = null;
            updateCycle.Index = -1;
            updateCycle.Update = null;
        }

        private static void OnDispose(IFixedUpdateCycle fixedUpdateCycle)
        {
            var index = fixedUpdateCycle.Index;
            if (index == -1) return;

            var list = fixedUpdateCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Dispose(index);
            list[index] = wrapper;

            fixedUpdateCycle.Wrappers = null;
            fixedUpdateCycle.Index = -1;
            fixedUpdateCycle.FixedUpdate = null;
        }

        private static void OnDispose(ILateUpdateCycle lateUpdateCycle)
        {
            var index = lateUpdateCycle.Index;
            if (index == -1) return;

            var list = lateUpdateCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Dispose(index);
            list[index] = wrapper;

            lateUpdateCycle.Wrappers = null;
            lateUpdateCycle.Index = -1;
            lateUpdateCycle.LateUpdate = null;
        }

        #endregion

        #region LifeCycleClass

        private class UCycle : LifeCycleBase, IUpdateCycle
        {
            public UCycle(IMonoBase monoBase) =>
                Update = (IUpdate)monoBase;

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IUpdateCycle.Wrappers { get; set; }
            int IUpdateCycle.Index { get; set; }
            public IUpdate Update { get; set; }
        }

        private class FCycle : LifeCycleBase, IFixedUpdateCycle
        {
            public FCycle(IMonoBase monoBase) =>
                FixedUpdate = (IFixedUpdate)monoBase;

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFixedUpdateCycle.Wrappers { get; set; }
            int IFixedUpdateCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }
        }

        private class LCycle : LifeCycleBase, ILateUpdateCycle
        {
            public LCycle(IMonoBase monoBase) =>
                LateUpdate = (ILateUpdate)monoBase;

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILateUpdateCycle.Wrappers { get; set; }
            int ILateUpdateCycle.Index { get; set; }
            public ILateUpdate LateUpdate { get; set; }
        }

        private class UFCycle : LifeCycleBase, IUpdateCycle, IFixedUpdateCycle
        {
            public UFCycle(IMonoBase monoBase)
            {
                Update = (IUpdate)monoBase;
                FixedUpdate = (IFixedUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IUpdateCycle.Wrappers { get; set; }
            int IUpdateCycle.Index { get; set; }
            public IUpdate Update { get; set; }

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFixedUpdateCycle.Wrappers { get; set; }
            int IFixedUpdateCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }
        }

        private class ULCycle : LifeCycleBase, IUpdateCycle, ILateUpdateCycle
        {
            public ULCycle(IMonoBase monoBase)
            {
                Update = (IUpdate)monoBase;
                LateUpdate = (ILateUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IUpdateCycle.Wrappers { get; set; }
            int IUpdateCycle.Index { get; set; }
            public IUpdate Update { get; set; }

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILateUpdateCycle.Wrappers { get; set; }
            int ILateUpdateCycle.Index { get; set; }
            public ILateUpdate LateUpdate { get; set; }
        }

        private class FLCycle : LifeCycleBase, IFixedUpdateCycle, ILateUpdateCycle
        {
            public FLCycle(IMonoBase monoBase)
            {
                FixedUpdate = (IFixedUpdate)monoBase;
                LateUpdate = (ILateUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFixedUpdateCycle.Wrappers { get; set; }
            int IFixedUpdateCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILateUpdateCycle.Wrappers { get; set; }
            int ILateUpdateCycle.Index { get; set; }
            public ILateUpdate LateUpdate { get; set; }
        }

        private class UFLCycle : LifeCycleBase, IUpdateCycle, IFixedUpdateCycle, ILateUpdateCycle
        {
            public UFLCycle(IMonoBase monoBase)
            {
                Update = (IUpdate)monoBase;
                FixedUpdate = (IFixedUpdate)monoBase;
                LateUpdate = (ILateUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IUpdateCycle.Wrappers { get; set; }
            int IUpdateCycle.Index { get; set; }
            public IUpdate Update { get; set; }

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFixedUpdateCycle.Wrappers { get; set; }
            int IFixedUpdateCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILateUpdateCycle.Wrappers { get; set; }
            int ILateUpdateCycle.Index { get; set; }
            public ILateUpdate LateUpdate { get; set; }
        }

        #endregion
    }

    internal class NullDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}