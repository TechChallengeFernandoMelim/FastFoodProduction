using System.Text.Json.Serialization;

namespace FastFoodProduction.Model;

public class Order
{
    [JsonPropertyName("pk")]
    public string Pk => InStoreOrderId;

    [JsonPropertyName("sk")]
    public string Sk => Pk;

    [JsonPropertyName("in_store_order_id")]
    public string InStoreOrderId { get; set; }

    [JsonPropertyName("itens_json")]
    public string ItensJson { get; set; }

    [JsonPropertyName("order_status")]
    public string OrderStatus { get; set; }

    [JsonPropertyName("receipt_handler_message")]
    public string ReceiptHandlerMessage { get; set; }
}
