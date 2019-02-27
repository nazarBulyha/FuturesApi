namespace FuturesApi.Models.Futures
{
    public interface IFutures
    {
        Currency Currency { get; set; }

        string ContractType { get; set; }
    }
}