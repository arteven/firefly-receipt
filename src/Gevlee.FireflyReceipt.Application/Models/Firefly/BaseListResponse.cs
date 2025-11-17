using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Models.Firefly
{
    public class BaseListResponse<TAttributes>
    {
        [JsonPropertyName("data")]
        public Datum<TAttributes>[] Data { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("links")]
        public ResponseLinks Links { get; set; }
    }

    public class BaseResponse<TAttributes>
    {
        [JsonPropertyName("data")]
        public Datum<TAttributes> Data { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("links")]
        public ResponseLinks Links { get; set; }
    }
}
