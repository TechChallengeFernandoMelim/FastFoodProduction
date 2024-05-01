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

    public async Task<bool> CreateOrder(Order order)
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
}
