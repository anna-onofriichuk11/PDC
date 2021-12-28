using System;
using TradingSystem.Common;

namespace TradingSystem.Models
{
    public class Order : IWithOrderId, IWithStockId
    {
        public string StockId { get; private set; }
        public Decimal StockQuantity { get; set; }
        public decimal StockPrice { get; set; }
        public Guid OrderId { get; }

        public Order(string stockId,  Guid orderId, decimal askPrice, decimal quantity)
        {
            StockId = stockId;
            OrderId = orderId;
            StockPrice = askPrice;
            StockQuantity = quantity;  
        }
    }
}
