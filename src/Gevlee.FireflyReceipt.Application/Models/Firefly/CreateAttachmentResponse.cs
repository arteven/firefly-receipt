using System;
using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Models.Firefly
{
    public class CreateAttachmentResponse : BaseResponse<CreateAttachmentResponse.Attributes>
    {
        public class Attributes
        {
            [JsonPropertyName("created_at")]
            public DateTimeOffset CreatedAt { get; set; }

            [JsonPropertyName("updated_at")]
            public DateTimeOffset UpdatedAt { get; set; }

            [JsonPropertyName("filename")]
            public string Filename { get; set; }

            [JsonPropertyName("attachable_type")]
            public string AttachableType { get; set; }

            [JsonPropertyName("attachable_id")]
            [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
            public long AttachableId { get; set; }

            [JsonPropertyName("md5")]
            public string Md5 { get; set; }

            [JsonPropertyName("download_uri")]
            public Uri DownloadUri { get; set; }

            [JsonPropertyName("upload_uri")]
            public Uri UploadUri { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("notes")]
            public string Notes { get; set; }

            [JsonPropertyName("mime")]
            public string Mime { get; set; }

            [JsonPropertyName("size")]
            [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
            public long Size { get; set; }
        }
    }
}
