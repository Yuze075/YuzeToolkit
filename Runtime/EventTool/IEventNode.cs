#nullable enable
using System.Collections.Generic;

namespace YuzeToolkit.EventTool
{
    public interface IEventNode
    {
        protected internal EventActionDictionary EventActions { get; }
        // todo 考虑如果良好的维护IEventInterceptor的关系
        protected internal IEnumerable<IEventInterceptor>? EventInterceptors => null;
        // todo 考虑如何结合节点树的结构去完成事件的传递(如何良好的进行向上传递事件
        // protected internal IEventNodeSender? NodeParent => null;
        // protected internal IEnumerable<IEventNodeSender>? NodeChildren => null;
    }
}