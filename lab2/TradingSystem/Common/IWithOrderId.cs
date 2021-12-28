using System;

namespace TradingSystem.Common
{
    interface IWithOrderId
    {
        Guid OrderId { get; }
    }
}
