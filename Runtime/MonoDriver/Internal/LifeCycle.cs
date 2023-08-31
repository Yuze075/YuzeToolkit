using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.Utility
{
    internal abstract class LifeCycleBase : ILifeCycle
    {
        private LifeCycleBase(IMonoBase monoBase, bool enable)
        {
            MonoBase = monoBase;
            _enable = enable;
        }

        private bool _enable;

        public bool Enable
        {
            get => _enable;
            set
            {
                if (value == _enable) return;
                _enable = value;
                if (_enable) OnEnable(this);
                else OnDisable(this);
            }
        }

        public IMonoBase MonoBase { get; private set; }

        public void Dispose()
        {
            OnDestroy(this);
            MonoBase = null;
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public static ILifeCycle Build(IMonoBase monoBase, bool enable)
        {
            return monoBase switch
            {
                IUpdate and IFixedUpdate and ILateUpdate => new UflLifeCycle(monoBase,enable),
                IUpdate and IFixedUpdate => new UfLifeCycle(monoBase,enable),
                IUpdate and ILateUpdate => new UlLifeCycle(monoBase,enable),
                IFixedUpdate and ILateUpdate => new FlLifeCycle(monoBase,enable),
                IUpdate => new ULifeCycle(monoBase,enable),
                IFixedUpdate => new FLifeCycle(monoBase,enable),
                ILateUpdate => new LLifeCycle(monoBase,enable),
                _ => new NullLifeCycle(monoBase,enable)
            };
        }

        #region LifeCycle

        private static void OnEnable(ILifeCycle lifeCycle)
        {
            if (lifeCycle is IULifeCycle uLifeCycle) UOnEnable(uLifeCycle);
            if (lifeCycle is IFLifeCycle fLifeCycle) FOnEnable(fLifeCycle);
            if (lifeCycle is ILLifeCycle lLifeCycle) LOnEnable(lLifeCycle);
        }

        private static void OnDisable(ILifeCycle lifeCycle)
        {
            if (lifeCycle is IULifeCycle uLifeCycle) UOnDisable(uLifeCycle);
            if (lifeCycle is IFLifeCycle fLifeCycle) FOnDisable(fLifeCycle);
            if (lifeCycle is ILLifeCycle lLifeCycle) LOnDisable(lLifeCycle);
        }

        private static void OnDestroy(ILifeCycle lifeCycle)
        {
            if (lifeCycle is IULifeCycle uLifeCycle) UOnDestroy(uLifeCycle);
            if (lifeCycle is IFLifeCycle fLifeCycle) FOnDestroy(fLifeCycle);
            if (lifeCycle is ILLifeCycle lLifeCycle) LOnDestroy(lLifeCycle);
        }

        private static void UOnEnable(IULifeCycle uLifeCycle)
        {
            var index = uLifeCycle.Index;
            if (index == -1) return;

            var list = uLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Enable();
            list[index] = wrapper;
        }

        private static void FOnEnable(IFLifeCycle fLifeCycle)
        {
            var index = fLifeCycle.Index;
            if (index == -1) return;

            var list = fLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Enable();
            list[index] = wrapper;
        }

        private static void LOnEnable(ILLifeCycle lLifeCycle)
        {
            var index = lLifeCycle.Index;
            if (index == -1) return;

            var list = lLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Enable();
            list[index] = wrapper;
        }

        private static void UOnDisable(IULifeCycle uLifeCycle)
        {
            var index = uLifeCycle.Index;
            if (index == -1) return;

            var list = uLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Disable();
            list[index] = wrapper;
        }

        private static void FOnDisable(IFLifeCycle fLifeCycle)
        {
            var index = fLifeCycle.Index;
            if (index == -1) return;

            var list = fLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Disable();
            list[index] = wrapper;
        }

        private static void LOnDisable(ILLifeCycle lLifeCycle)
        {
            var index = lLifeCycle.Index;
            if (index == -1) return;

            var list = lLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Disable();
            list[index] = wrapper;
        }

        private static void UOnDestroy(IULifeCycle uLifeCycle)
        {
            var index = uLifeCycle.Index;
            if (index == -1) return;

            var list = uLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Destroy(index);
            list[index] = wrapper;

            uLifeCycle.Wrappers = null;
            uLifeCycle.Index = -1;
            uLifeCycle.Update = null;
        }

        private static void FOnDestroy(IFLifeCycle fLifeCycle)
        {
            var index = fLifeCycle.Index;
            if (index == -1) return;

            var list = fLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Destroy(index);
            list[index] = wrapper;
            
            fLifeCycle.Wrappers = null;
            fLifeCycle.Index = -1;
            fLifeCycle.FixedUpdate = null;
        }

        private static void LOnDestroy(ILLifeCycle lLifeCycle)
        {
            var index = lLifeCycle.Index;
            if (index == -1) return;

            var list = lLifeCycle.Wrappers;
            var wrapper = list[index];
            wrapper.Destroy(index);
            list[index] = wrapper;
            
            lLifeCycle.Wrappers = null;
            lLifeCycle.Index = -1;
            lLifeCycle.LateUpdate = null;
        }

        #endregion

        #region LifeCycleClass
        
        private class NullLifeCycle : ILifeCycle
        {
            public NullLifeCycle(IMonoBase monoBase, bool enable)
            {
                MonoBase = monoBase;
                Enable = enable;
            }
            public void Dispose()
            { }

            public bool Enable { get; set; }
            public IMonoBase MonoBase { get; }
        }

        private class ULifeCycle : LifeCycleBase, IULifeCycle
        {
            public ULifeCycle(IMonoBase monoBase, bool enable) : base(monoBase, enable) =>
                Update = (IUpdate)monoBase;

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IULifeCycle.Wrappers { get; set; }
            int IULifeCycle.Index { get; set; }
            public IUpdate Update { get; set; }
        }

        private class FLifeCycle : LifeCycleBase, IFLifeCycle
        {
            public FLifeCycle(IMonoBase monoBase, bool enable ) : base(monoBase, enable) =>
                FixedUpdate = (IFixedUpdate)monoBase;

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFLifeCycle.Wrappers { get; set; }
            int IFLifeCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }
        }

        private class LLifeCycle : LifeCycleBase, ILLifeCycle
        {
            public LLifeCycle(IMonoBase monoBase, bool enable ) : base(monoBase, enable) =>
                LateUpdate = (ILateUpdate)monoBase;

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILLifeCycle.Wrappers { get; set; }
            int ILLifeCycle.Index { get; set; }
            public ILateUpdate LateUpdate { get; set; }
        }

        private class UfLifeCycle : LifeCycleBase, IULifeCycle, IFLifeCycle
        {
            public UfLifeCycle(IMonoBase monoBase, bool enable) : base(monoBase, enable)
            {
                Update = (IUpdate)monoBase;
                FixedUpdate = (IFixedUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IULifeCycle.Wrappers { get; set; }
            int IULifeCycle.Index { get; set; }
            public IUpdate Update{ get; set; }

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFLifeCycle.Wrappers { get; set; }
            int IFLifeCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }
        }

        private class UlLifeCycle : LifeCycleBase, IULifeCycle, ILLifeCycle
        {
            public UlLifeCycle(IMonoBase monoBase, bool enable) : base(monoBase, enable)
            {
                Update = (IUpdate)monoBase;
                LateUpdate = (ILateUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IULifeCycle.Wrappers { get; set; }
            int IULifeCycle.Index { get; set; }
            public IUpdate Update { get; set; }

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILLifeCycle.Wrappers { get; set; }
            int ILLifeCycle.Index { get; set; }
            public ILateUpdate LateUpdate{ get; set; }
        }

        private class FlLifeCycle : LifeCycleBase, IFLifeCycle, ILLifeCycle
        {
            public FlLifeCycle(IMonoBase monoBase, bool enable) : base(monoBase, enable)
            {
                FixedUpdate = (IFixedUpdate)monoBase;
                LateUpdate = (ILateUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFLifeCycle.Wrappers { get; set; }
            int IFLifeCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILLifeCycle.Wrappers { get; set; }
            int ILLifeCycle.Index { get; set; }
            public ILateUpdate LateUpdate{ get; set; }
        }

        private class UflLifeCycle : LifeCycleBase, IULifeCycle, IFLifeCycle, ILLifeCycle
        {
            public UflLifeCycle(IMonoBase monoBase, bool enable) : base(monoBase, enable)
            {
                Update = (IUpdate)monoBase;
                FixedUpdate = (IFixedUpdate)monoBase;
                LateUpdate = (ILateUpdate)monoBase;
            }

            IList<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> IULifeCycle.Wrappers { get; set; }
            int IULifeCycle.Index { get; set; }
            public IUpdate Update{ get; set; }

            IList<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> IFLifeCycle.Wrappers { get; set; }
            int IFLifeCycle.Index { get; set; }
            public IFixedUpdate FixedUpdate { get; set; }

            IList<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> ILLifeCycle.Wrappers { get; set; }
            int ILLifeCycle.Index { get; set; }
            public ILateUpdate LateUpdate { get; set; }
        }

        #endregion
    }
}