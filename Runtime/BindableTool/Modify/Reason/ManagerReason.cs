// using GamePlay.Frame.Manager;
//
// namespace GamePlay.Frame.BindableValue
// {
//     /// <summary>
//     /// <inheritdoc/>
//     /// 由<see cref="IManager"/>产生的修饰事件
//     /// </summary>
//     public readonly struct ManagerReason : IModifiableOwnerReason
//     {
//         public ManagerReason(IManager manager, IModifyReason? otherReason = null) =>
//             (Manager, OtherReason) = (manager, otherReason);
//
//         /// <summary>
//         /// 尝试进行修饰的组件
//         /// </summary>
//         public IManager Manager { get; }
//
//         /// <summary>
//         /// 其他修饰原因
//         /// </summary>
//         public IModifyReason? OtherReason { get; }
//
//         public IModifiableOwner OtherOwner => Manager;
//     }
// }