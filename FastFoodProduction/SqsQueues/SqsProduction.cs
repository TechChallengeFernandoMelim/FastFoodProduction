using Amazon.SQS;
using Amazon.SQS.Model;
using FastFoodProduction.Model;

namespace FastFoodProduction.SqsQueues;

public class SqsProduction(AmazonSQSClient sqsClient)
{
    private string sqsProductionQueue = Environment.GetEnvironmentVariable("AWS_SQS_PRODUCTION");

    public virtual async Task<Order> GetNextOrder()
    {
        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = sqsProductionQueue,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 10,
            MessageAttributeNames = new List<string> { "InStoreOrderId", "ItensJson", "PaymentStatus" }
        };

        var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);

        if (!receiveMessageResponse.Messages.Any())
            return null;

        var message = receiveMessageResponse.Messages[0];

        return new Order
        {
            InStoreOrderId = message.MessageAttributes["InStoreOrderId"].StringValue,
            ItensJson = message.MessageAttributes["ItensJson"].StringValue,
            OrderStatus = message.MessageAttributes["PaymentStatus"].StringValue,
            ReceiptHandlerMessage = message.ReceiptHandle
        };
    }

    public virtual async Task DeleteMessage(string receiptHandlerMessage)
    {
        var deleteMessageRequest = new DeleteMessageRequest
        {
            QueueUrl = Environment.GetEnvironmentVariable("AWS_SQS_PRODUCTION"),
            ReceiptHandle = receiptHandlerMessage
        };

        await sqsClient.DeleteMessageAsync(deleteMessageRequest);
    }
}
