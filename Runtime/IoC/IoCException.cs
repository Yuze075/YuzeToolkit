using System;

namespace YuzeToolkit.IoC
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