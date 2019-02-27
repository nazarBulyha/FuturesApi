namespace FuturesApi.Models.FuturesDepth
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class FuturesDepth : IFuturesDepth
    {
        [JsonProperty(PropertyName = "asks")]
        public List<FuturesDepthDetail> Asks { get; set; } = new List<FuturesDepthDetail>();

        [JsonProperty(PropertyName = "bids")]
        public List<FuturesDepthDetail> Bids { get; set; } = new List<FuturesDepthDetail>();
    }
}