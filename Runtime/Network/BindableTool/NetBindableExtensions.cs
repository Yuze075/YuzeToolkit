#nullable enable
#if YUZE_USE_UNITY_NETCODE
namespace YuzeToolkit.BindableTool.Network
{
    public static class NetBindableExtensions
    {
        #region IReadOnlyBindableRegister

        public static void AddBindable<TValue>(this IBindableNode self, NetBindableField<TValue> bindableField,
            IModifiableOwner modifiableOwner, TValue? value)
        {
            bindableField.SetOnly(value);
            self.AddBindable(bindableField, modifiableOwner);
        }

        public static void AddBindable<TValue>(this IBindableNode self, NetProperty<TValue> property,
            IModifiableOwner modifiableOwner, TValue valueBase)
            where TValue : unmanaged
        {
            property.SetOnly(valueBase, modifiableOwner);
            self.AddBindable(property, modifiableOwner);
        }

        public static void AddBindable<TValue>(this IBindableNode self, NetResource<TValue> resource,
            IModifiableOwner modifiableOwner, TValue value)
            where TValue : unmanaged
        {
            resource.SetOnly(value, modifiableOwner);
            self.AddBindable(resource, modifiableOwner);
        }

        public static void AddBindable(this IBindableNode self, NetState state, IModifiableOwner modifiableOwner,
            bool valueBase)
        {
            state.SetOnly(valueBase, modifiableOwner);
            self.AddBindable(state, modifiableOwner);
        }

        #endregion
    }
}
#endif