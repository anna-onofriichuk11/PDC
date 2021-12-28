using System;
using TradingSystem.Common;

namespace TradingSystem.Models
{
    public class Trade : Order
    {
        public OrderType OrderType { get; private set; }

        public DateTime CreateTime { get; private set; }
        public string Message { get; private set; }

        public Trade(string ticker, Guid orderId, decimal askPrice, OrderType tradeOrderType, decimal shares, string message = null)
            : base(ticker, orderId, askPrice, shares)
        {
            OrderType = tradeOrderType;
            CreateTime = DateTime.UtcNow;
            Message = message;
        }

        public Trade(string ticker, Guid orderId, decimal newPrice, OrderType tradeOrderType, string message = null)
            : base(ticker, orderId, newPrice, 0)
        {
            OrderType = tradeOrderType;           
            CreateTime = DateTime.UtcNow;            
            Message = message;
        }
        internal Order ToOrder()
        {
            return new Order(StockId, OrderId, StockPrice, StockQuantity);
        }

    }
}
