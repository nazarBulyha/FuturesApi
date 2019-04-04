namespace FuturesApi.Controllers
{
    using FuturesApi.Models.Futures;
    using FuturesApi.Models.FuturesDepth;
    using FuturesApi.Helpers;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using UtilHelper;
    using System.Web.Http.Cors;

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FuturesController : ApiController
    {
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

        // GET api/futures
        public IHttpActionResult GetFutures()
        {
            if (!Futureses.IsListOk())
            {
                return NotFound();
            }

            return Ok(Futureses);
        }

        // GET api/futures/1000
        public async Task<IHttpActionResult> GetFutures([FromUri]int value)
        {
            if (value == 0)
            {
                return BadRequest("Cumulative value can not be 0.");
            }

            #region Get data

            #region Create and fill tasks(get Future Depth)

            var tasks = new List<Task<IFuturesDepth>>();

            // convert data to that api want to see
            Futureses = FuturesHelper.ConvertToFromFuturesApi(Futureses);

            foreach (var futures in Futureses)
            {
                var futuresDepthTask = FuturesHelper.GetFutureDepth(futures);

                tasks.Add(futuresDepthTask);
            }

            // convert data from api variant
            Futureses = FuturesHelper.ConvertToFromFuturesApi(Futureses);

            #endregion

            #region Tasks execution result

            var futureDepthList = await Task.WhenAll(tasks);

            if (!futureDepthList.IsArrayOk())
            {
                return NotFound();
            }

            #endregion

            #endregion

            #region Core logic

            var updatedCurrencyValues = new List<string>();

            foreach (var futuresDepth in futureDepthList)
            {
                var askPrice = futuresDepth.Asks.FirstOrDefault(z => z.Cumulative >= value / z.Price);
                var bidPrice = futuresDepth.Bids.FirstOrDefault(z => z.Cumulative >= value / z.Price);

                var currencyValue = askPrice != null && bidPrice != null
                    ? Math.Round((askPrice.Price - bidPrice.Price) * 100 / askPrice.Price, 2) + "%"
                    : "0";

                updatedCurrencyValues.Add(currencyValue);
            }

            #endregion

            #region Update values

            var i = 0;
            foreach (var futures in Futureses)
            {
                futures.Currency.Value = updatedCurrencyValues[i];
                i++;
            }

            #endregion

            return Ok(Futureses);
        }
    }
}