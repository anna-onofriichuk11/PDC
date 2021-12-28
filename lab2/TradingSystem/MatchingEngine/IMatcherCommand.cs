using System.Collections.Generic;
using TradingSystem.Models;

namespace TradingSystem.MatchingEngine
{
    public interface IMatcherCommand
    {
        List<TradeTrasaction> ExecuteBuy(Order buyOrder);
        List<TradeTrasaction> ExecuteSell(Order sellOrder);
        TradeTrasaction ExecuteBuyPriceChange(Order pcOrder);
        TradeTrasaction ExecuteSellPriceChange(Order pcOrder);

        OrderBook OrderBook { get; }
    }
}