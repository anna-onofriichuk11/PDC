using Akka.Actor;
using TradingSystem.Common;
using TradingSystem.MatchingEngine;
using TradingSystem.Models;

namespace TradingSystem.Actors
{
    public class PriceChangeActor : ReceiveActor
    {
        private IMatcherCommand _matchingEngine;
        private readonly IActorRef _confirmationActor;
        
        public PriceChangeActor()
        {
            _matchingEngine = _matchingEngine ?? Matcher.Instance;
            _confirmationActor = Context.ActorOf(Props.Create(() => new StockBroker()));
            Receive<Trade>(trade => GetResponse(trade));
        }
       
        void PublishMessage(TradeTrasaction tradeTrasaction)
        {
            if (tradeTrasaction == null || tradeTrasaction.TradeStatus != TradeStatus.Listed)
                return;          
            
            _confirmationActor.Ask(tradeTrasaction); //publish event for price change
        }

        private void GetResponse(Trade trade)
        {
            TradeTrasaction tradeTrasaction = null;
            if(trade.OrderType == OrderType.Buy)
                tradeTrasaction = _matchingEngine.ExecuteBuyPriceChange(trade.ToOrder());            
            else if(trade.OrderType == OrderType.Sell)
                tradeTrasaction = _matchingEngine.ExecuteSellPriceChange(trade.ToOrder());
            PublishMessage(tradeTrasaction);
            Sender.Tell(tradeTrasaction, Self);
        }        
    }
}
