using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Models.Firefly
{
    public class Datum<TAttributes>
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonPropertyName("attributes")]
        public TAttributes Attributes { get; set; }

        [JsonPropertyName("links")]
        public Links Links { get; set; }
    }


    public class Links
    {
        [JsonPropertyName("0")]
        public Link Link { get; set; }

        [JsonPropertyName("self")]
        public string Self { get; set; }
    }

    public class Link
    {
        [JsonPropertyName("rel")]
        public string Rel { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

    public class ResponseLinks
    {
        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("first")]
        public string First { get; set; }

        [JsonPropertyName("prev")]
        public string Prev { get; set; }

        [JsonPropertyName("last")]
        public string Last { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        [JsonPropertyName("total")]
        public long Total { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }

        [JsonPropertyName("per_page")]
        public long PerPage { get; set; }

        [JsonPropertyName("current_page")]
        public long CurrentPage { get; set; }

        [JsonPropertyName("total_pages")]
        public long TotalPages { get; set; }
    }

    // System.Text.Json converter for parsing strings as longs
    public class ParseStringConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out long value))
                {
                    return value;
                }
                throw new JsonException("Cannot parse string as long");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt64();
            }
            throw new JsonException("Unexpected token type");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
