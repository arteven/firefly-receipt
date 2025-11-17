using System;
using System.Text.Json.Serialization;

namespace Gevlee.FireflyReceipt.Application.Models.Firefly
{
    public class GetTransactionsResponse : BaseListResponse<TransactionsAttributes>
    {
    }

    public class TransactionsAttributes
    {
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("user")]
        public long User { get; set; }

        [JsonPropertyName("group_title")]
        public string GroupTitle { get; set; }

        [JsonPropertyName("transactions")]
        public Transaction[] Transactions { get; set; }
    }

    public class Transaction
    {
        //[JsonPropertyName("user")]
        //public long User { get; set; }

        [JsonPropertyName("transaction_journal_id")]
        public long TransactionJournalId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        //[JsonPropertyName("date")]
        //public DateTimeOffset Date { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        //[JsonPropertyName("order")]
        //public long Order { get; set; }

        //[JsonPropertyName("currency_id")]
        //public long CurrencyId { get; set; }

        //[JsonPropertyName("currency_code")]
        //public string CurrencyCode { get; set; }

        //[JsonPropertyName("currency_symbol")]
        //public string CurrencySymbol { get; set; }

        [JsonPropertyName("currency_name")]
        public string CurrencyName { get; set; }

        //[JsonProperty("currency_decimal_places")]
        //public long CurrencyDecimalPlaces { get; set; }

        //[JsonProperty("foreign_amount")]
        //public decimal ForeignAmount { get; set; }

        //[JsonProperty("foreign_currency_id")]
        //public long ForeignCurrencyId { get; set; }

        //[JsonProperty("foreign_currency_code")]
        //public string ForeignCurrencyCode { get; set; }

        //[JsonProperty("foreign_currency_symbol")]
        //public string ForeignCurrencySymbol { get; set; }

        //[JsonProperty("foreign_currency_decimal_places")]
        //public long ForeignCurrencyDecimalPlaces { get; set; }

        //[JsonProperty("budget_id")]
        //public long BudgetId { get; set; }

        //[JsonProperty("budget_name")]
        //public string BudgetName { get; set; }

        //[JsonProperty("category_id")]
        //public long CategoryId { get; set; }

        //[JsonProperty("category_name")]
        //public string CategoryName { get; set; }

        //[JsonProperty("source_id")]
        //public long SourceId { get; set; }

        //[JsonProperty("source_name")]
        //public string SourceName { get; set; }

        //[JsonProperty("source_iban")]
        //public string SourceIban { get; set; }

        //[JsonProperty("source_type")]
        //public string SourceType { get; set; }

        //[JsonProperty("destination_id")]
        //public long DestinationId { get; set; }

        //[JsonProperty("destination_name")]
        //public string DestinationName { get; set; }

        //[JsonProperty("destination_iban")]
        //public string DestinationIban { get; set; }

        //[JsonProperty("destination_type")]
        //public string DestinationType { get; set; }

        //[JsonProperty("reconciled")]
        //public bool Reconciled { get; set; }

        //[JsonProperty("bill_id")]
        //public long BillId { get; set; }

        //[JsonProperty("bill_name")]
        //public string BillName { get; set; }

        //[JsonProperty("tags")]
        //public object Tags { get; set; }

        //[JsonProperty("notes")]
        //public string Notes { get; set; }

        //[JsonProperty("internal_reference")]
        //public string InternalReference { get; set; }

        //[JsonProperty("external_id")]
        //public string ExternalId { get; set; }

        //[JsonProperty("original_source")]
        //public string OriginalSource { get; set; }

        //[JsonProperty("recurrence_id")]
        //public long RecurrenceId { get; set; }

        //[JsonProperty("bunq_payment_id")]
        //public string BunqPaymentId { get; set; }

        //[JsonProperty("import_hash_v2")]
        //public string ImportHashV2 { get; set; }

        //[JsonProperty("sepa_cc")]
        //public string SepaCc { get; set; }

        //[JsonProperty("sepa_ct_op")]
        //public string SepaCtOp { get; set; }

        //[JsonProperty("sepa_ct_id")]
        //public string SepaCtId { get; set; }

        //[JsonProperty("sepa_db")]
        //public string SepaDb { get; set; }

        //[JsonProperty("sepa_country")]
        //public string SepaCountry { get; set; }

        //[JsonProperty("sepa_ep")]
        //public string SepaEp { get; set; }

        //[JsonProperty("sepa_ci")]
        //public string SepaCi { get; set; }

        //[JsonProperty("sepa_batch_id")]
        //public string SepaBatchId { get; set; }

        //[JsonProperty("interest_date")]
        //public DateTimeOffset InterestDate { get; set; }

        //[JsonProperty("book_date")]
        //public DateTimeOffset BookDate { get; set; }

        //[JsonProperty("process_date")]
        //public DateTimeOffset ProcessDate { get; set; }

        //[JsonProperty("due_date")]
        //public DateTimeOffset DueDate { get; set; }

        //[JsonProperty("payment_date")]
        //public DateTimeOffset PaymentDate { get; set; }

        //[JsonProperty("invoice_date")]
        //public DateTimeOffset InvoiceDate { get; set; }
    }
}
