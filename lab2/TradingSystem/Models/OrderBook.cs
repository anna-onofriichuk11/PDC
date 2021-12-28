using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingSystem.Models
{
    public class OrderBook
    {
        private List<Order> _bidOrders;
        private List<Order> _askOrders;
       
        public IReadOnlyList<Order> BidOrders
        {
            get
            {
                return _bidOrders.OrderByDescending(order => order.StockPrice).ToList();
            }
        }
     
        public IReadOnlyList<Order> AskOrders
        {
            get
            {
                return _askOrders.OrderBy(order => order.StockPrice).ToList();
            }
        }
     
        public OrderBook()
        {
            _bidOrders = new List<Order>();
            _askOrders = new List<Order>();
        }
        
        public void AddBidOrder(Order order)
        {
            var matchOrder = _bidOrders.Where(x => x.StockId == order.StockId).SingleOrDefault(x => order.StockPrice >= x.StockPrice);
            if (matchOrder != null)
                matchOrder.StockQuantity += order.StockQuantity;
            _bidOrders.Add(order);
        }
       
        public void AddAskOrder(Order order)
        {
            var matchOrder = _askOrders.Where(x => x.StockId == order.StockId).SingleOrDefault(x => order.StockPrice <= x.StockPrice);
            if (matchOrder != null)
                matchOrder.StockQuantity += order.StockQuantity;
            else
                _askOrders.Add(order);
        }
      
        public void RemoveBidOrder(Guid orderId)
        {            
            var removedOrder = _bidOrders.FirstOrDefault(p => p.OrderId == orderId);
            _bidOrders.Remove(removedOrder);
        }
     
        public void RemoveAskOrder(Guid orderId)
        {            
            var removedOrder = _askOrders.FirstOrDefault(p => p.OrderId == orderId);
            _askOrders.Remove(removedOrder);            
        }
               
    }
}
