#nullable enable
namespace YuzeToolkit.EventTool
{
    public interface IEventSystemOwner
    {
        EventSystem EventSystem { get; }
    }
}