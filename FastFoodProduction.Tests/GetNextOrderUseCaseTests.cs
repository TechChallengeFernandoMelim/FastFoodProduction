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

public class GetNextOrderUseCaseTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public GetNextOrderUseCaseTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    }

    [Fact]
    public async Task GetNextOrder_ValidOrder_Success()
    {
        // Arrange
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new GetNextOrderUseCase();

        var order = new Order
        {
            InStoreOrderId = "instore",
            OrderStatus = "Paid",
            ReceiptHandlerMessage = "receipt"
        };

        sqsProductionMock.Setup(x => x.GetNextOrder()).ReturnsAsync(order);
        productionRepositoryMock.Setup(x => x.CreateOrder(order)).ReturnsAsync(true);
        sqsProductionMock.Setup(x => x.DeleteMessage(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await useCase.GetNextOrder(loggerMock.Object, sqsProductionMock.Object, productionRepositoryMock.Object);

        // Assert
        Assert.Equal(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok<FastFoodProduction.Model.Order>)result).StatusCode);
        Assert.Equal(order.InStoreOrderId, ((Microsoft.AspNetCore.Http.HttpResults.Ok<FastFoodProduction.Model.Order>)result).Value.InStoreOrderId);
        //Assert.True(result.IsSuccess);
        //Assert.Equal(order, result.Value);
    }

    [Fact]
    public async Task GetNextOrder_NoOrder_ReturnsNoContent()
    {
        // Arrange
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new GetNextOrderUseCase();

        sqsProductionMock.Setup(x => x.GetNextOrder()).ReturnsAsync((Order)null);

        // Act
        var result = await useCase.GetNextOrder(loggerMock.Object, sqsProductionMock.Object, productionRepositoryMock.Object);

        // Assert
        Assert.Equal(204, ((Microsoft.AspNetCore.Http.HttpResults.NoContent)result).StatusCode);
        //Assert.True(result.IsNoContent);
    }

    [Fact]
    public async Task GetNextOrder_Exception_ReturnsBadRequestAndLogs()
    {
        // Arrange
        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);


        var exception = new Exception("Test Exception");

        sqsProductionMock.Setup(x => x.GetNextOrder()).ThrowsAsync(exception);
        loggerMock.
            Setup(x => x.Log(exception.StackTrace, exception.Message, exception.ToString()))
            .Callback((string stackTrace, string message, string exceptionMessage) =>
            {
                Console.WriteLine($"Logged exception:\nStackTrace: {stackTrace}\nMessage: {message}\nException: {exceptionMessage}");
            });

        // Act
        var useCase = new GetNextOrderUseCase();
        var result = await useCase.GetNextOrder(loggerMock.Object, sqsProductionMock.Object, productionRepositoryMock.Object);

        // Assert
        //Assert.True(result.IsBadRequest);
        Assert.Equal(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest)result).StatusCode);
        loggerMock.Verify(x => x.Log(exception.StackTrace, exception.Message, exception.ToString()), Times.Once);
    }
}
