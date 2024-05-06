using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using FastFoodProduction.Model;
using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;
using FastFoodProduction.UseCases;
using Moq;
using System;

namespace FastFoodProduction.Tests;

public class ChangeStatusUseCaseTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public ChangeStatusUseCaseTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    }

    [Fact]
    public async Task ChangeStatus_ValidInput_Success()
    {
        // Arrange
        var inStoreOrderId = "example_in_store_order_id";
        var newStatus = "Preparing";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        var order = new Order
        {
            InStoreOrderId = inStoreOrderId,
            OrderStatus = "Paid"
        };

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ReturnsAsync(order);
        productionRepositoryMock.Setup(x => x.ChangeOrderToStatus(inStoreOrderId, newStatus)).ReturnsAsync(true);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        Assert.Equal(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok)result).StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_ValidInput_BadRequest_DynamoError()
    {
        // Arrange
        var inStoreOrderId = "example_in_store_order_id";
        var newStatus = "Preparing";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        var order = new Order
        {
            InStoreOrderId = inStoreOrderId,
            OrderStatus = "Paid"
        };

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ReturnsAsync(order);
        productionRepositoryMock.Setup(x => x.ChangeOrderToStatus(inStoreOrderId, newStatus)).ReturnsAsync(false);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        loggerMock.Verify(x => x.Log(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ChangeStatus_ValidInput_PreparingToReady_Success()
    {
        // Arrange
        var inStoreOrderId = "example_in_store_order_id";
        var newStatus = "Ready";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        var order = new Order
        {
            InStoreOrderId = inStoreOrderId,
            OrderStatus = "Preparing"
        };

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ReturnsAsync(order);
        productionRepositoryMock.Setup(x => x.ChangeOrderToStatus(inStoreOrderId, newStatus)).ReturnsAsync(true);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        Assert.Equal(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok)result).StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_ValidInput_ReadyToFinished_Success()
    {
        // Arrange
        var inStoreOrderId = "example_in_store_order_id";
        var newStatus = "Finished";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        var order = new Order
        {
            InStoreOrderId = inStoreOrderId,
            OrderStatus = "Ready"
        };

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ReturnsAsync(order);
        productionRepositoryMock.Setup(x => x.ChangeOrderToStatus(inStoreOrderId, newStatus)).ReturnsAsync(true);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        Assert.Equal(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok)result).StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var inStoreOrderId = "example_in_store_order_id";
        var newStatus = "Finished";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        var order = new Order
        {
            InStoreOrderId = inStoreOrderId,
            OrderStatus = "Paid"
        };

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ReturnsAsync(order);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        Assert.Equal(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)result).StatusCode);
        Assert.Equal("Status inválido para o pedido selecionado.", ((Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)result).Value);
    }

    [Fact]
    public async Task ChangeStatus_OrderNotFound_ReturnsBadRequest()
    {
        // Arrange
        var inStoreOrderId = "non_existent_order_id";
        var newStatus = "Preparing";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ReturnsAsync((Order)null);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        Assert.Equal(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)result).StatusCode);
        Assert.Equal("Pedido não foi encontrado na base de dados.", ((Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>)result).Value);
    }

    [Fact]
    public async Task ChangeStatus_Exception_ReturnsBadRequestAndLogs()
    {
        // Arrange
        var inStoreOrderId = "example_in_store_order_id";
        var newStatus = "Preparing";

        var loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1).Object);
        var productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);

        var useCase = new ChangeStatusUseCase();

        var exception = new Exception("Test Exception");

        productionRepositoryMock.Setup(x => x.GetOrderByPk(inStoreOrderId)).ThrowsAsync(exception);
        loggerMock.Setup(x => x.Log(exception.StackTrace, exception.Message, exception.ToString())).Returns(Task.CompletedTask);

        // Act
        var result = await useCase.ChangeStatus(inStoreOrderId, loggerMock.Object, productionRepositoryMock.Object, newStatus);

        // Assert
        Assert.Equal(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest)result).StatusCode);
        loggerMock.Verify(x => x.Log(exception.StackTrace, exception.Message, exception.ToString()), Times.Once);
    }
}
