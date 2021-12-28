using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Persistence;
using TradingSystem.Common;
using TradingSystem.MatchingEngine;
using TradingSystem.Models;

namespace TradingSystem.Actors
{
    public class OrderBookActor : ReceivePersistentActor
    {
        public class GetMessages { }
        private List<TradeTrasaction> _msgs = new List<TradeTrasaction>(); 
        private IMatcherCommand _matchingEngine;
        private readonly IActorRef _confirmationActor;
        public override string PersistenceId => "OrderBook-Id";
    
        public OrderBookActor()
        {
            _matchingEngine = _matchingEngine ?? Matcher.Instance;
            _confirmationActor = Context.ActorOf(Props.Create(() => new StockBroker()));
                  
            Recovers();
            Commands();
        }
        
        public OrderBookActor(IMatcherCommand matchingEngine) : this()
        {
            _matchingEngine = matchingEngine;
            _confirmationActor = Context.ActorOf(Props.Create(() => new StockBroker()));
            Commands();
        }
  
        private void Recovers()
        {
            Recover<Trade>(trade => {
                if (trade.OrderType == OrderType.Sell)
                {
                    var tradeTrans = Ask(trade);
                    PublishMessage(tradeTrans);
                }
            });
            Recover<Trade>(trade => {
                if (trade.OrderType == OrderType.Buy)
                {
                    var tradeTrans = Bid(trade);
                    PublishMessage(tradeTrans);
                }
                
            });
            Recover<GetMessages> (get => Sender.Tell(_msgs));
        }
        
        private void Commands()
        {
            Command<Trade>(trade =>
            {
                if (trade.OrderType == OrderType.Sell)
                {
                    var tradeTrasactions = Ask(trade);
                    PublishMessage(tradeTrasactions);
                    Sender.Tell(tradeTrasactions);
                }
                else if (trade.OrderType == OrderType.Buy)
                {
                    var tradeTrasactions = Bid(trade);
                    PublishMessage(tradeTrasactions);
                    Sender.Tell(tradeTrasactions);
                }

            });
            Command<GetMessages>(get => {
                Sender.Tell(_msgs.OrderBy(x => x.CreateTime).ToList());
            });
        }
        
        List<TradeTrasaction> Ask(Trade sell)
        {            
            return _matchingEngine.ExecuteSell(sell.ToOrder());
        }
        List<TradeTrasaction> Bid(Trade buy)
        {            
            return _matchingEngine.ExecuteBuy(buy.ToOrder());
        }
        void PublishMessage(List<TradeTrasaction> tradeTrasactions)
        {
            tradeTrasactions.ForEach(trans =>
            {
                if (trans.TradeStatus == TradeStatus.PartiallyTraded || trans.TradeStatus == TradeStatus.Traded)
                    _confirmationActor.Ask(trans);
                _msgs.Add(trans);
            });
        }
    }
}
