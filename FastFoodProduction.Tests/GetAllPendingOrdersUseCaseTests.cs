using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using FastFoodProduction.Model;
using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;
using FastFoodProduction.UseCases;
using Moq;

namespace FastFoodProduction.Tests;

public class GetAllPendingOrdersUseCaseTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public GetAllPendingOrdersUseCaseTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    }

    [Fact]
    public async Task GetAllPendingOrders_ValidOrders_Success()
    {
        // Arrange
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new GetAllPendingOrdersUseCase();

        var orders = new List<Order>
            {
                new Order { },
                new Order { },
                new Order { }
            };

        productionRepositoryMock.Setup(x => x.GetPendingOrders()).ReturnsAsync(orders);

        // Act
        var result = await useCase.GetAllPendingOrders(loggerMock.Object, productionRepositoryMock.Object);

        // Assert
        //Assert.True(result.IsSuccess);
        Assert.Equal(orders.Count, ((Microsoft.AspNetCore.Http.HttpResults.Ok<System.Collections.Generic.List<FastFoodProduction.Model.Order>>)result).Value.Count);
    }

    [Fact]
    public async Task GetAllPendingOrders_Exception_ReturnsBadRequestAndLogs()
    {
        // Arrange
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new GetAllPendingOrdersUseCase();

        var exception = new Exception("Test Exception");

        productionRepositoryMock.Setup(x => x.GetPendingOrders()).ThrowsAsync(exception);
        loggerMock.Setup(x => x.Log(exception.StackTrace, exception.Message, exception.ToString())).Returns(Task.CompletedTask);

        // Act
        var result = await useCase.GetAllPendingOrders(loggerMock.Object, productionRepositoryMock.Object);

        // Assert
        Assert.Equal(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest)result).StatusCode);
        loggerMock.Verify(x => x.Log(exception.StackTrace, exception.Message, exception.ToString()), Times.Once);
    }
}
