// using GamePlay.Frame.Entity_Component;
//
// namespace GamePlay.Frame.BindableValue
// {
//     /// <summary>
//     /// <inheritdoc/>
//     /// 由<see cref="IComponent"/>产生的修饰事件
//     /// </summary>
//     public readonly struct ComponentReason : IModifiableOwnerReason
//     {
//         public ComponentReason(IComponent component, IModifyReason? otherReason = null) =>
//             (Component, OtherReason) = (component, otherReason);
//
//         /// <summary>
//         /// 尝试进行修饰的组件
//         /// </summary>
//         public IComponent Component { get; }
//
//         /// <summary>
//         /// 其他修饰原因
//         /// </summary>
//         public IModifyReason? OtherReason { get; }
//
//         public IModifiableOwner OtherOwner => Component;
//     }
// }