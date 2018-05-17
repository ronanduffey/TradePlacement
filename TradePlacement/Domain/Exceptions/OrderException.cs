using System;

namespace TradePlacement.Domain.Exceptions
{
    public abstract class OrderException : Exception
    {
        public string ExceptionCode { get; }

        public OrderException(string exceptionCode, string message) : base(message)
        {
            ExceptionCode = exceptionCode;
        }
    }
}