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
        void Register<T>(T t) where T : IBindable;

        static IBindableRegister RegisterNull { get; } = new NullBindableRegister();

        private class NullBindableRegister : IBindableRegister
        {
            public void Register<T>(T t) where T : IBindable
            {
            }
        }
    }
}