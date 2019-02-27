namespace FuturesApi.UtilHelper
{
    using FuturesApi.Models;
    using System.Collections.Generic;

    public static class Constants
    {
        public static List<string> CurrencyLabels = new List<string>
        {
            CurrencyLabel.BTC,
            CurrencyLabel.LTC,
            CurrencyLabel.ETH,
            CurrencyLabel.ETC,
            CurrencyLabel.BCH,
            CurrencyLabel.XRP,
            CurrencyLabel.EOS
        };

        public static List<string> ContractTypes = new List<string>
        {
            ContractType.ThisWeek,
            ContractType.Quarter,
            ContractType.NextWeek
        };
    }
}