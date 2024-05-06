using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodProduction.SqsQueues;
using Moq;

namespace FastFoodProduction.Tests;

public class SqsProductionTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public SqsProductionTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    }

    [Fact]
    public async Task OrderQueue_ValidInput_Success()
    {
        // Arrange 
        var sqsClientMock = new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1);

        sqsClientMock
            .Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse()
            {
                Messages = new List<Message>()
                {
                    new Message()
                    {
                        MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                        {
                            {"InStoreOrderId", new MessageAttributeValue(){ StringValue = "InStoreOrderId"} },
                            {"ItensJson", new MessageAttributeValue(){ StringValue = "ItensJson" } }
                        },
                        ReceiptHandle = "teste"
                    }
                }
            });

        var sqsProduction = new SqsProduction(sqsClientMock.Object);

        // Act
        var order = await sqsProduction.GetNextOrder();

        // Assert
        Assert.Equal("InStoreOrderId", order.InStoreOrderId);
        Assert.Equal("ItensJson", order.ItensJson);
        Assert.Equal("teste", order.ReceiptHandlerMessage);
    }

    [Fact]
    public async Task DeleteMessage_ValidInput_Success()
    {
        // Arrange
        var sqsClientMock = new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1);
        var sqsProduction = new SqsProduction(sqsClientMock.Object);
        string receiptHandlerMessage = "testReceiptHandle";

        // Act
        await sqsProduction.DeleteMessage(receiptHandlerMessage);

        // Assert
        sqsClientMock.Verify(x => x.DeleteMessageAsync(It.Is<DeleteMessageRequest>(req => req.ReceiptHandle == receiptHandlerMessage), It.IsAny<CancellationToken>()), Times.Once);
    }
}
