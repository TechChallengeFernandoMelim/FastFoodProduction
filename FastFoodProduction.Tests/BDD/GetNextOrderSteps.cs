using Amazon.DynamoDBv2;
using Amazon.SQS;
using FastFoodProduction.Model;
using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;
using FastFoodProduction.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechTalk.SpecFlow;

namespace FastFoodProduction.Tests.BDD;

[Binding]
public class GetNextOrderSteps
{
    private readonly Mock<SqsLogger> loggerMock = new Mock<SqsLogger>(new Mock<AmazonSQSClient>().Object);
    private readonly Mock<SqsProduction> sqsProductionMock = new Mock<SqsProduction>(new Mock<AmazonSQSClient>().Object);
    private readonly Mock<ProductionRepository> productionRepositoryMock = new Mock<ProductionRepository>(new Mock<IAmazonDynamoDB>().Object);
    private GetNextOrderUseCase useCase;
    private Order retrievedOrder;
    private IResult result;

    [Given(@"there is a valid order available in the messaging service")]
    public void GivenThereIsValidOrderAvailableInTheMessagingService()
    {
        var order = new Order
        {
            InStoreOrderId = "instore",
            OrderStatus = "Paid",
            ReceiptHandlerMessage = "receipt"
        };

        sqsProductionMock.Setup(x => x.GetNextOrder()).ReturnsAsync(order);
        productionRepositoryMock.Setup(x => x.CreateOrder(order)).ReturnsAsync(true);
        sqsProductionMock.Setup(x => x.DeleteMessage(It.IsAny<string>())).Returns(Task.CompletedTask);
    }

    [When(@"an employee attempts to retrieve the next order")]
    public async Task WhenAnEmployeeAttemptsToRetrieveTheNextOrder()
    {
        useCase = new GetNextOrderUseCase();
        result = await useCase.GetNextOrder(loggerMock.Object, sqsProductionMock.Object, productionRepositoryMock.Object);
    }

    [Then(@"the system should return the order details")]
    public void ThenTheSystemShouldReturnTheOrderDetails()
    {
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<Order>>(result);
        var okResult = (Microsoft.AspNetCore.Http.HttpResults.Ok<Order>)result;
        retrievedOrder = okResult.Value;
        Assert.NotNull(retrievedOrder);
    }

    [Then(@"the order status should indicate it's paid")]
    public void ThenTheOrderStatusShouldIndicateItsPaid()
    {
        Assert.Equal("Paid", retrievedOrder.OrderStatus);
    }
}
