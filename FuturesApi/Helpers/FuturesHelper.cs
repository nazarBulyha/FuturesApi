using System.Collections.Generic;

namespace FuturesApi.UtilHelper
{
    using FuturesApi.Models;
    using FuturesApi.Models.Futures;
    using FuturesApi.Models.FuturesDepth;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public static class FuturesHelper
    {
        // send api request to okex and fill FutureDepth model
        // for current Currency and Contract Type
        public static async Task<IFuturesDepth> GetFutureDepth(IFutures future)
        {
            var url = HttpHelper.CreateUrl(future.Currency.Name, future.ContractType);
            var requestMethod = "GET";
            var data = await HttpHelper.TryGetData(url, requestMethod);

            var multiplier = future.Currency.Name == "btc_usd" ? 10 : 1;
            var futureDepth = ParseFutures(data, multiplier);

            return futureDepth;
        }

        public static IFuturesDepth ParseFutures(string data, int multiplier)
        {
            var apiResult = JObject.Parse(data);
            IFuturesDepth futureDepth = new FuturesDepth();

            foreach (var itemList in apiResult)
            {
                var reverseAsks = itemList.Value.Reverse();
                double totalCumulative = 0;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (itemList.Key == "asks")
                {
                    foreach (var item in reverseAsks)
                    {
                        var amount = Math.Round(item.Last.Value<double>() * 10 * multiplier / item.First.Value<double>(), 5);
                        var ask = new FuturesDepthDetail
                        {
                            Price = item.First.Value<double>(),
                            Amount = amount,
                            Cumulative = Math.Round(totalCumulative + amount, 5)
                        };

                        totalCumulative = ask.Cumulative;
                        futureDepth.Asks.Add(ask);
                    }
                }
                else if (itemList.Key == "bids")
                {
                    foreach (var item in itemList.Value)
                    {
                        var amount = Math.Round(item.Last.Value<double>() * 10 * multiplier / item.First.Value<double>(), 5);
                        var bid = new FuturesDepthDetail
                        {
                            Price = item.First.Value<double>(),
                            Amount = amount,
                            Cumulative = Math.Round(totalCumulative + amount, 5)
                        };

                        totalCumulative = bid.Cumulative;
                        futureDepth.Bids.Add(bid);
                    }
                }
            }

            return futureDepth;
        }

        public static List<IFutures> ConvertToFromFuturesApi(List<IFutures> futureses)
        {
            var alreadyContains = futureses.Select(futures => futures.Currency.Name)
                                           .First()
                                           .Contains("_usd");

            foreach (var futurese in futureses)
            {
                futurese.Currency.Name = alreadyContains
                    ? (futurese.Currency.Name.Replace("_usd", "")).ToUpper()
                    : (futurese.Currency.Name + "_usd").ToLower();
            }

            return futureses;
        }
    }
}