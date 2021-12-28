using System;
using TradingSystem.Common;

namespace TradingSystem.Models
{  
    public class TradeTrasaction : IWithOrderId, IWithStockId
    {
        public string StockId { get; private set;  }
        public Guid OrderId { get; private set; }
        public Decimal OrderPrice { get; set; }
        public  Decimal OrderQuantity { get; set; }
        public OrderType  OrderType{ get; private set;}
        public TradeStatus TradeStatus { get; private set; }
        
        public DateTime CreateTime { get; private set; }

        public TradeTrasaction(string stockId, TradeStatus tradeStatus, OrderType orderType, Decimal price, Decimal quantity, Guid orderId)
        {
            StockId = stockId;
            TradeStatus = tradeStatus;
            OrderType = orderType;
            OrderPrice = price;
            OrderQuantity = quantity;
            OrderId = orderId;
            CreateTime = DateTime.Now;
        }

       
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", OrderType, OrderId, OrderType);
        }
    }
}
