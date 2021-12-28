using System;
using System.Collections.Generic;
using System.Linq;
using TradingSystem.Common;
using TradingSystem.Models;

namespace TradingSystem.MatchingEngine
{
    public sealed class Matcher : IMatcherCommand
    {
        static volatile Lazy<Matcher> _instance = new Lazy<Matcher>(() => new Matcher(), true);
        public static Matcher Instance => _instance.Value;
       
        private Matcher() {
            OrderBook = OrderBook ?? new OrderBook();
        }
    
        public OrderBook OrderBook { get; }      
       
        public Matcher(OrderBook orderBook)
        {
            OrderBook = orderBook;
        }
    
        public List<TradeTrasaction> ExecuteBuy(Order buyOrder)
        {
            if (buyOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var trades = new List<TradeTrasaction>();            
           
            var matchingAskOrders = OrderBook.AskOrders.Where(p => p.StockId == buyOrder.StockId && buyOrder.StockPrice >= p.StockPrice).OrderByDescending(stock => stock.StockPrice).ToList();
            foreach (var askOrder in matchingAskOrders)
            {   
                if (askOrder.StockQuantity >= buyOrder.StockQuantity)
                {                    
                    askOrder.StockQuantity -= buyOrder.StockQuantity;
                    if (askOrder.StockQuantity == 0)
                    {
                        trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.Traded, OrderType.Buy, buyOrder.StockPrice, buyOrder.StockQuantity, buyOrder.OrderId));
                        OrderBook.RemoveAskOrder(askOrder.OrderId);                        

                    }
                    else                    
                        trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.Traded, OrderType.Buy, buyOrder.StockPrice, buyOrder.StockQuantity, buyOrder.OrderId));
                    buyOrder.StockQuantity = 0;
                }
                
                else if (askOrder.StockQuantity < buyOrder.StockQuantity)
                {
                    buyOrder.StockQuantity -= askOrder.StockQuantity;
                    OrderBook.RemoveAskOrder(askOrder.OrderId);
                    trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.PartiallyTraded, OrderType.Buy, buyOrder.StockPrice, askOrder.StockQuantity, buyOrder.OrderId));
                }                
            }
            if (buyOrder.StockQuantity == 0)
                return trades;
            
            OrderBook.AddBidOrder(buyOrder);
            trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.Listed, OrderType.Buy, buyOrder.StockPrice, buyOrder.StockQuantity, buyOrder.OrderId));
            return trades;
        }
     
        public List<TradeTrasaction> ExecuteSell(Order sellOrder)
        {
            if (sellOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var trades = new List<TradeTrasaction>();
            var matchingBidOrders = OrderBook.BidOrders.Where(p => p.StockId == sellOrder.StockId && p.StockPrice >= sellOrder.StockPrice).OrderByDescending(stock => stock.StockPrice).ToList();
            foreach (var bidOrder in matchingBidOrders)
            {   
                if (bidOrder.StockQuantity >= sellOrder.StockQuantity)
                {                    
                    bidOrder.StockQuantity -= sellOrder.StockQuantity;
                    if (bidOrder.StockQuantity == 0)
                    {
                        trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.Traded, OrderType.Sell, sellOrder.StockPrice, sellOrder.StockQuantity, sellOrder.OrderId));
                        OrderBook.RemoveBidOrder(bidOrder.OrderId);
                        
                    }
                    else
                        trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.Traded, OrderType.Sell, sellOrder.StockPrice, bidOrder.StockQuantity, sellOrder.OrderId));
                    sellOrder.StockQuantity = 0;
                }
                else if (bidOrder.StockQuantity < sellOrder.StockQuantity)
                {                    
                    sellOrder.StockQuantity -= bidOrder.StockQuantity;
                    OrderBook.RemoveBidOrder(bidOrder.OrderId);
                    trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.PartiallyTraded, OrderType.Sell, sellOrder.StockPrice, sellOrder.StockQuantity, sellOrder.OrderId));
                }               
            }
            if (sellOrder.StockQuantity == 0 )
                return trades;
            OrderBook.AddAskOrder(sellOrder);
            trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.Listed, OrderType.Sell, sellOrder.StockPrice, sellOrder.StockQuantity, sellOrder.OrderId));
            return trades;
        }

        public TradeTrasaction ExecuteBuyPriceChange(Order pcOrder)
        {
            if (pcOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var order = OrderBook.BidOrders.FirstOrDefault(p => p.OrderId == pcOrder.OrderId);
            if (order != null)
            {
                order.StockPrice = pcOrder.StockPrice;
                return new TradeTrasaction(order.StockId, TradeStatus.Listed, OrderType.Buy, order.StockPrice, order.StockQuantity, order.OrderId);
                
            }
            return null;
        }

        public TradeTrasaction ExecuteSellPriceChange(Order pcOrder)
        {
            if (pcOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var order = OrderBook.AskOrders.FirstOrDefault(p => p.OrderId == pcOrder.OrderId);
            if (order != null)
            {
                order.StockPrice = pcOrder.StockPrice;                
                return new TradeTrasaction(order.StockId, TradeStatus.Listed, OrderType.Sell, order.StockPrice, order.StockQuantity, order.OrderId);
            }
            return null;
        }
    }
}
