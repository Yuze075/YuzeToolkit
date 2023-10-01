namespace YuzeToolkit.IoC
{
    public class ValueWrapper<T> : IValueWrapper
    {
        public ValueWrapper(T value) => _value = value;

        private readonly T _value;

        public bool TryGet<TValue>(out TValue tValue)
        {
            if (_value is TValue t)
            {
                tValue = t;
                return true;
            }

            tValue = default;
            return false;
        }
    }
}