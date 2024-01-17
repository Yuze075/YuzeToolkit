#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <see cref="IBindable"/>的注册器
    /// </summary>
    public interface IBindableRegister
    {
        /// <summary>
        /// 注册<see cref="IBindable"/>
        /// </summary>
        void AddBindable<T>(T bindable) where T : IBindable;

        static IBindableRegister RegisterNull { get; } = new NullBindableRegister();

        private class NullBindableRegister : IBindableRegister
        {
            public void AddBindable<T>(T bindable) where T : IBindable
            {
            }
        }
    }
}