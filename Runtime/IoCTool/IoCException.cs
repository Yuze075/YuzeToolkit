using System;

namespace YuzeToolkit.IoCTool
{
    public class IoCException : Exception
    {
        public IoCException()
        {
        }

        public IoCException(string message) : base(message)
        {
        }
    }
}