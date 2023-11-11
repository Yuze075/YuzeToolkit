using YuzeToolkit.IoCTool;

namespace YuzeToolkit.DataTool
{
    public static class DataExtend
    {
        public static void RegisterData<TData, TValue>(this Container container, TData data)
            where TData : StringData<TValue> where TValue : IModel<string>
        {
            ((IData)data).Parent = container;
            container.Register(data).As<StringData<TValue>, IData<TValue>>();
        }

        public static void RegisterData<TData, TValue, TId>(this Container container, TData data)
            where TData : Data<TValue, TId> where TValue : IModel<TId> where TId : unmanaged
        {
            ((IData)data).Parent = container;
            container.Register(data).As<Data<TValue, TId>, IData<TValue>>();
        }

        public static void RegisterData<TData, TValue>(this Container container)
            where TData : StringData<TValue>, new() where TValue : IModel<string>
        {
            var data = new TData();
            ((IData)data).Parent = container;
            container.Register(data).As<StringData<TValue>, IData<TValue>>();
        }

        public static void RegisterData<TData, TValue, TId>(this Container container)
            where TData : Data<TValue, TId>, new() where TValue : IModel<TId> where TId : unmanaged
        {
            var data = new TData();
            ((IData)data).Parent = container;
            container.Register(data).As<Data<TValue, TId>, IData<TValue>>();
        }
    }
}