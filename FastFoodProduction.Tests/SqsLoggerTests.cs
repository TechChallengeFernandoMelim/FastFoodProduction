using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodProduction.SqsQueues;
using Moq;

namespace FastFoodProduction.Tests;

public class SqsLoggerTests
{
    Mock<BasicAWSCredentials> _credentialsMock;

    public SqsLoggerTests()
    {
        _credentialsMock = new Mock<BasicAWSCredentials>("accesskey", "secretekeys");
    }

    [Fact]
    public async Task Log_ValidInput_Success()
    {
        // Arrange
        var stackTrace = "Test stack trace";
        var message = "Test message";
        var exception = "Test exception";

        var sqsClientMock = new Mock<AmazonSQSClient>(_credentialsMock.Object, RegionEndpoint.USEast1);
        sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new SendMessageResponse());

        var sqsService = new SqsLogger(sqsClientMock.Object);

        // Act
        await sqsService.Log(stackTrace, message, exception);

        // Assert
        sqsClientMock.Verify(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
