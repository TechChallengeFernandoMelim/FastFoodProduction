using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using FastFoodProduction.Model;
using FastFoodProduction.Repositories;
using Moq;
using System.Net;

namespace FastFoodProduction.Tests;

public class ProductionRepositoryTests
{
    Mock<IAmazonDynamoDB> _dynamoDbMock;

    public ProductionRepositoryTests()
    {
        _dynamoDbMock = new Mock<IAmazonDynamoDB>();
    }

    [Fact]
    public async Task CreateOrder_ValidInput_Success()
    {
        // Arrange
        var order = new Order { /* Adicione propriedades de exemplo */ };
        var repository = new ProductionRepository(_dynamoDbMock.Object);

        _dynamoDbMock.Setup(x => x.PutItemAsync(It.IsAny<PutItemRequest>(), default))
                     .ReturnsAsync(new PutItemResponse { HttpStatusCode = HttpStatusCode.OK });

        // Act
        var result = await repository.CreateOrder(order);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetOrderByPk_ValidInput_Success()
    {
        // Arrange
        var repository = new ProductionRepository(_dynamoDbMock.Object);
        var inStoreOrderId = "example_in_store_order_id";

        _dynamoDbMock.Setup(x => x.ScanAsync(It.IsAny<ScanRequest>(), default))
                     .ReturnsAsync(new ScanResponse { Items = new List<Dictionary<string, AttributeValue>>() });

        // Act
        var result = await repository.GetOrderByPk(inStoreOrderId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPendingOrders_ValidInput_Success()
    {
        // Arrange
        var repository = new ProductionRepository(_dynamoDbMock.Object);

        _dynamoDbMock.Setup(x => x.ScanAsync(It.IsAny<ScanRequest>(), default))
                     .ReturnsAsync(new ScanResponse { Items = new List<Dictionary<string, AttributeValue>>() });

        // Act
        var result = await repository.GetPendingOrders();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ChangeOrderToStatus_ValidInput_Success()
    {
        // Arrange
        var repository = new ProductionRepository(_dynamoDbMock.Object);
        var inStoreOrderId = "example_in_store_order_id";
        var status = "example_status";

        _dynamoDbMock.Setup(x => x.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), default))
                     .ReturnsAsync(new UpdateItemResponse { HttpStatusCode = HttpStatusCode.OK });

        // Act
        var result = await repository.ChangeOrderToStatus(inStoreOrderId, status);

        // Assert
        Assert.True(result);
    }
}
