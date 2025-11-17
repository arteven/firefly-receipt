using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Models.Firefly
{
    public class CreateAttachmentRequest
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("attachable_type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttachableType AttachableType { get; set; }

        [JsonPropertyName("attachable_id")]
        public string AttachableId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }
    }
}
