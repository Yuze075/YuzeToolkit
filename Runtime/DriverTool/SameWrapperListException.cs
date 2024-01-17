#nullable enable
using System;

namespace YuzeToolkit.DriverTool
{
    public class SameWrapperListException : Exception
    {
        public SameWrapperListException()
        {
        }

        public SameWrapperListException(string message) : base(message)
        {
        }

        public SameWrapperListException(int priority, Type type) : base($"已经存在相同的优先级{priority}的{type}!")
        {
        }
    }
}