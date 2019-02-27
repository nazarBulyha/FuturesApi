namespace FuturesApi.Models.Futures
{
    public class Futures : IFutures
    {
        public Currency Currency { get; set; }

        public string ContractType { get; set; }
    }
}