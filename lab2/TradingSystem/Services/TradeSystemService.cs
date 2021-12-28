using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using TradingSystem.Actors;
using TradingSystem.Common;
using TradingSystem.Models;
using static TradingSystem.Actors.OrderBookActor;

namespace TradingSystem.Services
{
    public interface ITradeSystemService
    {
        List<TradeTrasaction> Trade(string ticker, Guid orderId, decimal price, decimal shares, OrderType orderSide);
        TradeTrasaction TradePriceUpdate(string ticker, Guid orderId, decimal newPrice, OrderType ordertype);
        IReadOnlyList<TradeTrasaction> OrderTransactionsHistory { get; }
    }
 
    public class TradeSystemService : ITradeSystemService
    {
        private readonly IActorRef _orderBookActor;
      
        private readonly IActorRef _priceChageActor;

        public TradeSystemService(ActorSystem actorSystem)
        {
            _orderBookActor = actorSystem.ActorOf(Props.Create<OrderBookActor>(), "OrderBookActor");
            _priceChageActor = actorSystem.ActorOf(Props.Create<PriceChangeActor>(), "PriceChangeActor");
        }
 
        public List<TradeTrasaction> Trade(string ticker, Guid orderId, decimal price, decimal shares, OrderType orderSide)
        {
          
            var transMessage = new Trade(ticker, orderId, price, orderSide, shares);
           
            var task = AskFromOrderBookActor(transMessage);
           
            task.Wait();
        
            return task.Result;
        }

        public TradeTrasaction TradePriceUpdate(string ticker, Guid orderId, decimal newPrice, OrderType ordertype)
        {
            var transMessage = new Trade(ticker, orderId, newPrice, ordertype);
           
            var task = AskFromPriceChangeActor(transMessage);
            
            task.Wait();
          
            return task.Result;
        }
     
        public IReadOnlyList<TradeTrasaction> OrderTransactionsHistory
        {
            get
            {
                var task = GetTransMessages();
                task.Wait();
                return task.Result;
            }
        }

        private Task<List<TradeTrasaction>> GetTransMessages()
        {
            return _orderBookActor.Ask<List<TradeTrasaction>>(new GetMessages());
        }
        
        private Task<List<TradeTrasaction>> AskFromOrderBookActor(Trade trade)
        {
            return _orderBookActor.Ask<List<TradeTrasaction>>(trade);  
        }
        private Task<TradeTrasaction> AskFromPriceChangeActor(Trade trade)
        {
            return _priceChageActor.Ask<TradeTrasaction>(trade);            
        }
    }
}
