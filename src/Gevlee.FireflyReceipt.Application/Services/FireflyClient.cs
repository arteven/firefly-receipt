using Gevlee.FireflyReceipt.Application.Models.Firefly;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gevlee.FireflyReceipt.Application.Services
{
    public class FireflyClient : IFireflyClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public FireflyClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<GetAttachmentsResponse> GetAttachmentsAsync(int? page = null, int? limit = null)
        {
            try
            {
                var url = "api/v1/attachments?";
                if (page.HasValue)
                    url += $"page={page}";
                if (limit.HasValue)
                    url += $"&limit={limit}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GetAttachmentsResponse>(_jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GetTransactionsResponse> GetTransactionsAsync(int? page = null, int? limit = null, DateTime? start = null, DateTime? end = null, string type = null)
        {
            try
            {
                var url = "api/v1/transactions?";
                if (page.HasValue)
                    url += $"page={page}";
                if (limit.HasValue)
                    url += $"&limit={limit}";
                if (start.HasValue)
                    url += $"&start={start.Value:yyyy-MM-dd}";
                if (end.HasValue)
                    url += $"&end={end.Value:yyyy-MM-dd}";
                if (!string.IsNullOrEmpty(type))
                    url += $"&type={Uri.EscapeDataString(type)}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GetTransactionsResponse>(_jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateAttachmentResponse> CreateAttachmentAsync(CreateAttachmentRequest requestModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/attachments", requestModel, _jsonOptions);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<CreateAttachmentResponse>(_jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UploadAttachment(long attachmentId, byte[] fileBytes)
        {
            try
            {
                var url = $"api/v1/attachments/{attachmentId}/upload";
                var content = new ByteArrayContent(fileBytes);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                var response = await _httpClient.PostAsync(url, content);

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception("Invalid response code != 204");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
