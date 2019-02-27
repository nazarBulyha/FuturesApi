namespace FuturesApi.Controllers
{
    using FuturesApi.Models.Futures;
    using FuturesApi.Models.FuturesDepth;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using UtilHelper;

    public class FuturesController : ApiController
    {
        // api = toLower + _usd
        private List<IFutures> Futureses { get; set; }

        public FuturesController()
        {
            Futureses = new List<IFutures>();

            // initialize Futures model with default values
            foreach (var currencyLabel in Constants.CurrencyLabels)
            {
                foreach (var contractType in Constants.ContractTypes)
                {
                    Futureses.Add(new Futures
                    {
                        Currency = new Currency
                        {
                            Name = currencyLabel,
                            Value = "0"
                        },
                        ContractType = contractType
                    });
                } 
            }
        }

        [HttpGet]
        public IHttpActionResult GetFutures()
        {
            if (Futureses == null || Futureses.Count > 0)
            {
                return NotFound();
            }

            var result = JsonConvert.SerializeObject(Futureses);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetFutures(int? cumulative)
        {
            if (!cumulative.HasValue)
            {
                return NotFound();
            }

	        var tasks = new List<Task<IFuturesDepth>>();
            var updatedCurrencyValues = new List<string>();

            foreach (var futures in Futureses)
            {
                var futuresDepthTask = FuturesHelper.GetFutureDepth(futures);

                tasks.Add(futuresDepthTask);
            }

            var futureDepthList = await Task.WhenAll(tasks);

            if (futureDepthList.Length < 0)
            {
                return NotFound();
            }

            foreach (var futuresDepth in futureDepthList)
            {
                #region Core logic

                var askPrice = futuresDepth.Asks.FirstOrDefault(z => z.Cumulative >= cumulative / z.Price);
                var bidPrice = futuresDepth.Bids.FirstOrDefault(z => z.Cumulative >= cumulative / z.Price);

                var currencyValue = askPrice != null && bidPrice != null
                    ? Math.Round((askPrice.Price - bidPrice.Price) * 100 / askPrice.Price, 2) + "%"
                    : "0";

                updatedCurrencyValues.Add(currencyValue);

                #endregion
            }
            
            // update values
            var i = 0;
            foreach (var futures in Futureses)
            {
                futures.Currency.Value = updatedCurrencyValues[i];
                i++;
            }

            return Ok(Futureses);
        }
    }
}