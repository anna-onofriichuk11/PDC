using Akka.Actor;
using Akka.Event;
using TradingSystem.Common;
using TradingSystem.Models;

namespace TradingSystem.Actors
{
    public class StockBroker : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();
        public StockBroker()
        {            
            Receive<TradeTrasaction>(tradeTrasaction => GetResponse(tradeTrasaction)); //Completed transactions
        }
      
        private void GetResponse(TradeTrasaction tradeTrasaction)
        {            
            if (tradeTrasaction.TradeStatus == TradeStatus.Listed)
                log.Info($"Trade is Listed with Price {tradeTrasaction.OrderPrice}");
            if (tradeTrasaction.TradeStatus == TradeStatus.PartiallyTraded)
                log.Info($"Partially Traded {tradeTrasaction.OrderQuantity}");
            if (tradeTrasaction.TradeStatus == TradeStatus.Traded)
                log.Info("Trade is successful");            

            Sender.Tell(tradeTrasaction);
        }        
    }
}
