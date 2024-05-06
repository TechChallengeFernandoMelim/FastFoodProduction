using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FastFoodProduction.Model;
using System.Net;
using System.Text.Json;

namespace FastFoodProduction.Repositories;

public class ProductionRepository(IAmazonDynamoDB dynamoDb)
{
    private static string tableName = Environment.GetEnvironmentVariable("AWS_TABLE_NAME_DYNAMO");

    public virtual async Task<bool> CreateOrder(Order order)
    {
        var orderAsJson = JsonSerializer.Serialize(order);
        var itemAsDocument = Document.FromJson(orderAsJson);
        var itemAsAttribute = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = tableName,
            Item = itemAsAttribute
        };

        var response = await dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public virtual async Task<Order> GetOrderByPk(string in_store_order_id)
    {
        var request = new ScanRequest
        {
            TableName = tableName,
            FilterExpression = "pk = :pk",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":pk", new AttributeValue { S = in_store_order_id } }
                }
        };

        var response = await dynamoDb.ScanAsync(request);

        if (response.Items.Count == 0)
            return null;

        var itemAsDocument = Document.FromAttributeMap(response.Items.First());
        return JsonSerializer.Deserialize<Order>(itemAsDocument.ToJson());
    }

    public virtual async Task<List<Order>> GetPendingOrders()
    {
        var queryRequest = new ScanRequest
        {
            TableName = tableName,
            FilterExpression = "order_status = :order_status",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":order_status", new AttributeValue { S = "Paid" } }
            }
        };

        var response = await dynamoDb.ScanAsync(queryRequest);

        var orders = response.Items.Select(item =>
        {
            var order = new Order
            {
                InStoreOrderId = item["in_store_order_id"].S,
                ItensJson = item["itens_json"].S,
                OrderStatus = item["order_status"].S,
                ReceiptHandlerMessage = item["receipt_handler_message"].S
            };
            return order;
        }).ToList();

        return orders;
    }

    public virtual async Task<bool> ChangeOrderToStatus(string in_store_order_id, string status)
    {
        var updateItemRequest = new UpdateItemRequest
        {
            TableName = tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = in_store_order_id } },
                { "sk", new AttributeValue { S = in_store_order_id } }
            },
            AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
            {
                {
                    "order_status", new AttributeValueUpdate
                    {
                        Action = AttributeAction.PUT,
                        Value = new AttributeValue { S = status }
                    }
                }
            }
        };

        var response = await dynamoDb.UpdateItemAsync(updateItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
