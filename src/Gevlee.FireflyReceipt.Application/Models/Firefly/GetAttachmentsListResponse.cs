using System;
using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Models.Firefly
{
    public class GetAttachmentsResponse : BaseListResponse<AttachmentsAttributes>
    {
    }

    public class AttachmentsAttributes
    {
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("attachable_id")]
        public string AttachableId { get; set; }

        [JsonPropertyName("attachable_type")]
        public string AttachableType { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("download_uri")]
        public string DownloadUri { get; set; }

        [JsonPropertyName("upload_uri")]
        public string UploadUri { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("mime")]
        public string Mime { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }

}
