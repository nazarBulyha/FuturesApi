namespace FuturesApi.Models.FuturesDepth
{
    using System.Collections.Generic;

    public interface IFuturesDepth
    {
        List<FuturesDepthDetail> Asks { get; set; }

        List<FuturesDepthDetail> Bids { get; set; }
    }
}