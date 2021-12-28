using System;
using Akka.Actor;
using NUnit.Framework;
using TradingSystem.Common;
using TradingSystem.MatchingEngine;
using TradingSystem.Models;
using TradingSystem.Services;

namespace TradingSystemTests
{
    [TestOf(typeof(TradeSystemService))]    
    public class TradingSystemTests
    {
        private TradeSystemService tradeSystem;
        private IMatcherCommand _matchingEngine;

        [SetUp]
        public void Setup()
        {
            tradeSystem = new TradeSystemService(ActorSystem.Create("TradingSystem-App-Test"));
            _matchingEngine = Matcher.Instance;
        }
        [Test]
        public void Test_Full_Trade_Order()
        {            
            // Arrange            
            var bidOrder1 = new Order("AO", Guid.NewGuid(), 100, 3);
            var bidOrder2 = new Order("AO", Guid.NewGuid(), 101, 2);
            var askOrder1 = new Order("AO", Guid.NewGuid(), 101, 7);
            var bidOrder3 = new Order("AO", Guid.NewGuid(), 100, 2);

            // Act 
            tradeSystem.Trade("AO", bidOrder1.OrderId, 100, 2, OrderType.Sell);
            tradeSystem.Trade("AO", bidOrder2.OrderId, 101, 3, OrderType.Sell);  
            tradeSystem.Trade("AO", askOrder1.OrderId, 101, 7, OrderType.Buy);            
            tradeSystem.Trade("AO", bidOrder3.OrderId, 100, 2, OrderType.Sell);

            //Read
            var transMessages = tradeSystem.OrderTransactionsHistory;
            var orderBook = _matchingEngine.OrderBook;
            
            // Assert
            Assert.AreEqual(transMessages.Count, 6);

            Assert.AreEqual(orderBook.BidOrders.Count, 0);
            Assert.AreEqual(orderBook.AskOrders.Count, 0);
            Assert.AreEqual(transMessages[0].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[0].OrderId, bidOrder1.OrderId);
            Assert.AreEqual(transMessages[1].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[1].OrderId, bidOrder2.OrderId);
            Assert.AreEqual(transMessages[2].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[2].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[3].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[3].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[4].OrderType, OrderType.Buy); //Remain 2 listed for buying
            Assert.AreEqual(transMessages[4].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[5].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[5].OrderId, bidOrder3.OrderId); 
        }
        [Test]
        public void Test_Partial_Trade_Order()
        {
            // Arrange            
            var bidOrder1 = new Order("AO", Guid.NewGuid(), 100, 3);
            var bidOrder2 = new Order("AO", Guid.NewGuid(), 101, 2);            
            var askOrder1 = new Order("AO", Guid.NewGuid(), 101, 4);
            var askOrder2 = new Order("AO", Guid.NewGuid(), 100, 7);

            // Act 
            tradeSystem.Trade(bidOrder1.StockId, bidOrder1.OrderId, bidOrder1.StockPrice, bidOrder1.StockQuantity, OrderType.Sell);
            tradeSystem.Trade(bidOrder2.StockId, bidOrder2.OrderId, bidOrder2.StockPrice, bidOrder2.StockQuantity, OrderType.Sell);
            tradeSystem.Trade(askOrder1.StockId, askOrder1.OrderId, askOrder1.StockPrice, askOrder1.StockQuantity, OrderType.Buy);
            tradeSystem.Trade(askOrder2.StockId, askOrder2.OrderId, askOrder2.StockPrice, askOrder2.StockQuantity, OrderType.Buy);

            //Read
            var transMessages = tradeSystem.OrderTransactionsHistory;
            var orderBook = _matchingEngine.OrderBook;

            // Assert
            Assert.AreEqual(transMessages.Count, 6);

            Assert.AreEqual(orderBook.BidOrders.Count, 1); //  askOrder2 - 6
            Assert.AreEqual(orderBook.AskOrders.Count, 0); // bidOrder2 - 0
            Assert.AreEqual(transMessages[0].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[0].OrderId, bidOrder1.OrderId);
            Assert.AreEqual(transMessages[1].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[1].OrderId, bidOrder2.OrderId);
            Assert.AreEqual(transMessages[2].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[2].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[3].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[3].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[4].OrderType, OrderType.Buy); 
            Assert.AreEqual(transMessages[4].OrderId, askOrder2.OrderId);
            Assert.AreEqual(transMessages[5].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[5].OrderId, askOrder2.OrderId);            
        }
        [Test]
        public void Test_Full_PriceChange_Trade_Order()
        {                    
            // Arrange            
            var bidOrder1 = new Order("AO", Guid.NewGuid(), 100, 3);
            var bidOrder2 = new Order("AO", Guid.NewGuid(), 101, 4);
            var askOrder1 = new Order("AO", Guid.NewGuid(), 101, 2);
            var askOrder2 = new Order("AO", Guid.NewGuid(), 102, 5);

            // Act 
            tradeSystem.Trade(bidOrder1.StockId, bidOrder1.OrderId, bidOrder1.StockPrice, bidOrder1.StockQuantity, OrderType.Sell);
            tradeSystem.Trade(bidOrder2.StockId, bidOrder2.OrderId, bidOrder2.StockPrice, bidOrder2.StockQuantity, OrderType.Sell);
            tradeSystem.Trade(askOrder1.StockId, askOrder1.OrderId, askOrder1.StockPrice, askOrder1.StockQuantity, OrderType.Buy);
            tradeSystem.TradePriceUpdate(bidOrder1.StockId, bidOrder2.OrderId, 102, OrderType.Sell); //trigger the price change - 102
            tradeSystem.Trade(askOrder2.StockId, askOrder2.OrderId, askOrder2.StockPrice, askOrder2.StockQuantity, OrderType.Buy);


            //Read
            var transMessages = tradeSystem.OrderTransactionsHistory;
            var orderBook = _matchingEngine.OrderBook;

            // Assert
            // Assert
            //  Assert.AreEqual(transMessages.Count, 6);

            Assert.AreEqual(orderBook.BidOrders.Count, 0);
            Assert.AreEqual(orderBook.AskOrders.Count, 0);
            Assert.AreEqual(transMessages[0].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[0].OrderId, bidOrder1.OrderId);
            Assert.AreEqual(transMessages[1].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[1].OrderId, bidOrder2.OrderId);
            Assert.AreEqual(transMessages[2].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[2].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[3].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[3].OrderId, askOrder1.OrderId);
            Assert.AreEqual(transMessages[4].OrderType, OrderType.Buy); 
            Assert.AreEqual(transMessages[4].OrderId, askOrder2.OrderId);
            Assert.AreEqual(transMessages[5].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[5].OrderId, askOrder2.OrderId);
        }

        [Test]
        public void Test_Partially_PriceChange_Trade_Order()
        {
            // Arrange            
            var bidOrder1 = new Order("AO", Guid.NewGuid(), 100, 4);
            var bidOrder2 = new Order("AO", Guid.NewGuid(), 101, 5);
            var askOrder1 = new Order("AO", Guid.NewGuid(), 101, 2);
            var askOrder2 = new Order("AO", Guid.NewGuid(), 102, 5);

            // Act 
            tradeSystem.Trade(bidOrder1.StockId, bidOrder1.OrderId, bidOrder1.StockPrice, bidOrder1.StockQuantity, OrderType.Sell);
            tradeSystem.Trade(bidOrder2.StockId, bidOrder2.OrderId, bidOrder2.StockPrice, bidOrder2.StockQuantity, OrderType.Sell);
            tradeSystem.Trade(askOrder1.StockId, askOrder1.OrderId, askOrder1.StockPrice, askOrder1.StockQuantity, OrderType.Buy);
            tradeSystem.TradePriceUpdate(bidOrder2.StockId, bidOrder2.OrderId, 103, OrderType.Sell); //trigger the price change - 103
            tradeSystem.Trade(askOrder2.StockId, askOrder2.OrderId, askOrder2.StockPrice, askOrder2.StockQuantity, OrderType.Buy);


            //Read
            var transMessages = tradeSystem.OrderTransactionsHistory;
            var orderBook = _matchingEngine.OrderBook;

            // Assert
            // Assert
            Assert.AreEqual(transMessages.Count, 4);

            Assert.AreEqual(orderBook.BidOrders.Count, 2); // bidorder2 - 1, price - 103
            Assert.AreEqual(orderBook.AskOrders.Count, 1); // askorder2 - 1, price - 102
            Assert.AreEqual(transMessages[0].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[0].OrderId, bidOrder1.OrderId);
            Assert.AreEqual(transMessages[1].OrderType, OrderType.Sell);
            Assert.AreEqual(transMessages[1].OrderId, bidOrder2.OrderId);
            Assert.AreEqual(transMessages[2].OrderType, OrderType.Buy);
            Assert.AreEqual(transMessages[2].OrderId, askOrder1.OrderId);
        }
    }
}
