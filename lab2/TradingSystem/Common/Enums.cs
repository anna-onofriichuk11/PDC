namespace TradingSystem.Common
{
    public enum OrderType
    {
        Buy,
        Sell          
    }
    
    public enum TradeStatus
    {
        None,
        Listed,
        Traded,
        PartiallyTraded,
        NotTraded,
        Invalid
    }
}
